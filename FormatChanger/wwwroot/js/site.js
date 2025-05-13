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
        if (response.ok) {
            window.location.href = `/Home/Export`;
        } else {
            alert('Ошибка при отправке запроса');
        }
    })
    .catch(error => alert('Ошибка сети:', error));
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