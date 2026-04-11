let currentIndex = 0;
let paragraphs = [];
let typeOptions = [];

// Отобразить форму загрузки
function showUploadForm() {
    var form = document.getElementById('uploadForm');
    form.style.display = 'block';
}

// Начать процесс форматирования
function startFormattingProcess() {
    var selectedTemplateId = document.getElementById('templateSelect').value;
    var selectedActionId = document.getElementById('actionSelect').value;
    const paragraphData = getParagraphTypes();

    fetch(`/Home/StartFormattingProcess?templateId=${selectedTemplateId}&actionId=${selectedActionId}`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(paragraphData)
    })
    .then(response => {
        if (!response.ok)
            throw new Error("Ошибка при процессе форматирования документа");
        return fetch(`/Home/Export`);
        })
    .then(response => {
        if (response.headers.get('content-disposition')?.includes('attachment')) {
            window.location.href = '/Home/Export';
        } else {
            return response.json();
        }
    })
    .then(data => {
        if (data)
            alert(data.message || "Успешно");
    })
    .catch(error => alert('Ошибка при экспорте:', error));
}

// Получить типы абзацев
function getParagraphTypes() {
    return Array.from(document.querySelectorAll('.text-block'))
        .map(paragraph => paragraph.dataset.type);
}

// Обработать загрузку страницы
document.addEventListener('DOMContentLoaded', function () {
    paragraphs = document.querySelectorAll('#paragraphsContainer .text-block');
    typeOptions = document.querySelectorAll('.type-option');

    if (paragraphs.length > 0) {
        updateActiveParagraph();
    }

    setupParagraphNavigation();
    setupTypeSelection();
});

// Обеспечить навигацию между абзацами
function setupParagraphNavigation() {
    // Назначить активным кликнутый абзац
    paragraphs.forEach((paragraph, index) => {
        paragraph.addEventListener('click', function () {
            currentIndex = index;
            updateActiveParagraph();
        });
    });

    // Назначить активным предыдущий абзац
    document.querySelector('.prev-btn').addEventListener('click', function () {
        if (currentIndex > 0) {
            currentIndex--;
            updateActiveParagraph();
        }
    });

    // Назначить активным следующий абзац
    document.querySelector('.next-btn').addEventListener('click', function () {
        if (currentIndex < paragraphs.length - 1) {
            currentIndex++;
            updateActiveParagraph();
        }
    });

    // Обработать навигацию по абзацам стрелками
    document.addEventListener('keydown', function (event) {
        if (event.key === 'ArrowUp' || event.key === 'ArrowLeft') {
            if (currentIndex > 0) {
                currentIndex--;
                updateActiveParagraph();
            }
            event.preventDefault();
        }

        if (event.key === 'ArrowDown' || event.key === 'ArrowRight') {
            if (currentIndex < paragraphs.length - 1) {
                currentIndex++;
                updateActiveParagraph();
            }
            event.preventDefault();
        }
    });
}

// Сделать абзац активным
function updateActiveParagraph() {
    paragraphs.forEach((p, index) => {
        p.classList.toggle('highlighted', index === currentIndex);
        if (index === currentIndex) {
            updateActiveTypeButton(p);
        }
    });
}

// Обеспечить выбор типа абзаца
function setupTypeSelection() {
    typeOptions.forEach(option => {
        option.addEventListener('click', function () {
            const activeParagraph = document.querySelector('.text-block.highlighted');
            activeParagraph.dataset.type = option.getAttribute('data-type');
            updateActiveTypeButton(activeParagraph);
        });
    });
}

// Обновить выбранный тип абзаца
function updateActiveTypeButton(paragraph) {
    const type = paragraph.dataset.type;
    typeOptions.forEach(option => {
        option.classList.toggle('active', option.getAttribute('data-type') === type);
    });
}

// ========== Управление уровнями заголовков в модальном окне ==========
// Эти функции работают с динамически создаваемыми карточками заголовков

function reindexCards() {
    const cards = document.querySelectorAll('#headings-list .heading-card');
    cards.forEach((card, idx) => {
        const headerSpan = card.querySelector('.card-header span');
        if (headerSpan) headerSpan.innerText = `Заголовок уровня ${idx + 1}`;

        const hiddenLevel = card.querySelector('.heading-level');
        if (hiddenLevel) hiddenLevel.value = idx + 1;

        const settingsTitle = card.querySelector('.card-body h6');
        if (settingsTitle) settingsTitle.innerText = `Настройки текста заголовка уровня ${idx + 1}`;

        card.querySelectorAll('[name]').forEach(el => {
            let oldName = el.getAttribute('name');
            let newName = oldName.replace(/HeadingLevelsEdit\[\d+\]/, `HeadingLevelsEdit[${idx}]`);
            el.setAttribute('name', newName);
        });

        const removeBtn = card.querySelector('.remove-heading');
        if (idx === 0) {
            if (removeBtn) removeBtn.remove();
        } else {
            if (!removeBtn) {
                const header = card.querySelector('.card-header');
                const btn = document.createElement('button');
                btn.type = 'button';
                btn.className = 'btn-close remove-heading';
                btn.setAttribute('aria-label', 'Удалить');
                header.appendChild(btn);
            }
        }
    });
    attachRemoveHandlers();
}

function attachRemoveHandlers() {
    document.querySelectorAll('.remove-heading').forEach(btn => {
        btn.removeEventListener('click', removeHandler);
        btn.addEventListener('click', removeHandler);
    });
}

function removeHandler(e) {
    const card = e.target.closest('.heading-card');
    if (!card) return;
    const totalCards = document.querySelectorAll('.heading-card').length;
    if (totalCards <= 1) {
        alert('Нельзя удалить единственный уровень заголовка');
        return;
    }
    card.remove();
    reindexCards();
}

function initHeadingsEditor() {
    // Проверяем, что контейнер с карточками существует
    const container = document.getElementById('headings-list');
    if (!container) return;

    // Если нет ни одной карточки, возможно, нужно создать дефолтную (но обычно сервер отдаёт хотя бы одну)
    if (document.querySelectorAll('.heading-card').length === 0) {
        // Можно добавить заглушку, но лучше чтобы сервер всегда отдавал хотя бы один уровень
        return;
    }

    reindexCards();

    // Навешиваем обработчик на кнопку добавления (если она есть)
    const addBtn = document.getElementById('add-heading-btn');
    if (addBtn) {
        addBtn.removeEventListener('click', addHeadingHandler);
        addBtn.addEventListener('click', addHeadingHandler);
    }
}

function addHeadingHandler() {
    const container = document.getElementById('headings-list');
    const currentCards = document.querySelectorAll('.heading-card');
    const newIndex = currentCards.length;
    const newId = -Date.now(); // временный отрицательный ID

    const firstCard = document.querySelector('.heading-card');
    if (!firstCard) return;

    const clone = firstCard.cloneNode(true);
    // Очищаем значения полей
    clone.querySelectorAll('input, select, textarea').forEach(input => {
        if (input.type === 'checkbox' || input.type === 'radio') {
            input.checked = false;
        } else if (input.type === 'color') {
            input.value = '#000000';
        } else {
            input.value = '';
        }
    });
    const startNewPageChk = clone.querySelector('.start-new-page');
    if (startNewPageChk) startNewPageChk.checked = false;

    const hiddenId = clone.querySelector('.heading-id');
    if (hiddenId) hiddenId.value = newId;
    const hiddenLevel = clone.querySelector('.heading-level');
    if (hiddenLevel) hiddenLevel.value = newIndex + 1;

    const span = clone.querySelector('.card-header span');
    if (span) span.innerText = `Заголовок уровня ${newIndex + 1}`;
    const titleH6 = clone.querySelector('.card-body h6');
    if (titleH6) titleH6.innerText = `Настройки текста заголовка уровня ${newIndex + 1}`;

    container.appendChild(clone);
    reindexCards();
}

// Инициализация при открытии модального окна (Bootstrap)
const modalElement = document.getElementById('formattingTemplateModal');
if (modalElement) {
    modalElement.addEventListener('shown.bs.modal', function () {
        initHeadingsEditor();
    });
}

// Также инициализируем при переключении на вкладку "Заголовки" (на случай, если содержимое подгружается позже)
const headingsTab = document.querySelector('button[data-bs-target="#headings"]');
if (headingsTab) {
    headingsTab.addEventListener('shown.bs.tab', function () {
        initHeadingsEditor();
    });
}