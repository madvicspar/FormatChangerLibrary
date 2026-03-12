let currentIndex = 0;
let paragraphs = [];
let typeOptions = [];
let currentDocumentId = null; // сохраняем ID текущего документа

// Показать форму загрузки
document.getElementById('showUploadButton').addEventListener('click', function() {
    document.getElementById('uploadForm').style.display = 'block';
});

// Обработка отправки формы загрузки
document.getElementById('uploadForm').addEventListener('submit', async function(e) {
    e.preventDefault();
    const formData = new FormData();
    const fileInput = document.getElementById('file');
    formData.append('file', fileInput.files[0]);

    try {
        const response = await fetch('/api/documents/upload', {
            method: 'POST',
            body: formData
        });
        if (!response.ok) throw new Error('Ошибка загрузки файла');
        const data = await response.json();
        currentDocumentId = data.documentId;
        renderParagraphs(data.paragraphs);
        // Скрыть форму после загрузки
        document.getElementById('uploadForm').style.display = 'none';
    } catch (error) {
        alert('Ошибка: ' + error.message);
    }
});

// Отрисовка параграфов
function renderParagraphs(paragraphsData) {
    const container = document.getElementById('paragraphsContainer');
    container.innerHTML = '';
    paragraphsData.forEach((p, index) => {
        const pElem = document.createElement('p');
        pElem.className = 'text-block';
        pElem.dataset.index = index;
        pElem.dataset.type = p.type;
        pElem.textContent = p.innerText; // предполагаем, что API возвращает innerText
        container.appendChild(pElem);
    });
    paragraphs = document.querySelectorAll('#paragraphsContainer .text-block');
    typeOptions = document.querySelectorAll('.type-option');
    if (paragraphs.length > 0) {
        currentIndex = 0;
        updateActiveParagraph();
    }
    setupParagraphNavigation();
    setupTypeSelection();
}

// Навигация по абзацам (аналогично старому коду)
function setupParagraphNavigation() {
    paragraphs.forEach((p, index) => {
        p.addEventListener('click', () => {
            currentIndex = index;
            updateActiveParagraph();
        });
    });

    document.querySelector('.prev-btn').addEventListener('click', () => {
        if (currentIndex > 0) {
            currentIndex--;
            updateActiveParagraph();
        }
    });

    document.querySelector('.next-btn').addEventListener('click', () => {
        if (currentIndex < paragraphs.length - 1) {
            currentIndex++;
            updateActiveParagraph();
        }
    });

    document.addEventListener('keydown', (e) => {
        if (e.key === 'ArrowUp' || e.key === 'ArrowLeft') {
            if (currentIndex > 0) {
                currentIndex--;
                updateActiveParagraph();
            }
            e.preventDefault();
        }
        if (e.key === 'ArrowDown' || e.key === 'ArrowRight') {
            if (currentIndex < paragraphs.length - 1) {
                currentIndex++;
                updateActiveParagraph();
            }
            e.preventDefault();
        }
    });
}

function updateActiveParagraph() {
    paragraphs.forEach((p, index) => {
        p.classList.toggle('highlighted', index === currentIndex);
        if (index === currentIndex) {
            updateActiveTypeButton(p);
        }
    });
}

function setupTypeSelection() {
    typeOptions.forEach(option => {
        option.addEventListener('click', () => {
            const activeParagraph = document.querySelector('.text-block.highlighted');
            if (!activeParagraph) return;
            const newType = option.getAttribute('data-type');
            activeParagraph.dataset.type = newType;
            updateActiveTypeButton(activeParagraph);
        });
    });
}

function updateActiveTypeButton(paragraph) {
    const type = paragraph.dataset.type;
    typeOptions.forEach(option => {
        option.classList.toggle('active', option.getAttribute('data-type') === type);
    });
}

// Загрузка шаблонов при старте
async function loadTemplates() {
    try {
        const response = await fetch('/api/templates');
        if (!response.ok) throw new Error('Не удалось загрузить шаблоны');
        const templates = await response.json();
        const select = document.getElementById('templateSelect');
        select.innerHTML = '';
        templates.forEach(t => {
            const option = document.createElement('option');
            option.value = t.id;
            option.textContent = t.title;
            select.appendChild(option);
        });
    } catch (error) {
        console.error('Ошибка загрузки шаблонов:', error);
    }
}

// Запуск форматирования и экспорт
document.getElementById('exportButton').addEventListener('click', async function() {
    if (!currentDocumentId) {
        alert('Сначала загрузите документ.');
        return;
    }

    const templateId = document.getElementById('templateSelect').value;
    const actionId = document.getElementById('actionSelect').value;
    const types = Array.from(document.querySelectorAll('.text-block')).map(p => p.dataset.type);

    try {
        // POST /api/documents/format
        const formatResponse = await fetch(`/api/documents/format?templateId=${templateId}&actionId=${actionId}&documentId=${currentDocumentId}`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(types)
        });
        if (!formatResponse.ok) throw new Error('Ошибка при форматировании');

        // GET /api/documents/export/{documentId}
        const exportResponse = await fetch(`/api/documents/export/${currentDocumentId}`);
        if (exportResponse.headers.get('content-disposition')?.includes('attachment')) {
            // Если это файл — перенаправляем или создаём ссылку для скачивания
            const blob = await exportResponse.blob();
            const url = window.URL.createObjectURL(blob);
            const a = document.createElement('a');
            a.href = url;
            a.download = 'document.docx'; // или получить имя из заголовка
            document.body.appendChild(a);
            a.click();
            a.remove();
            window.URL.revokeObjectURL(url);
        } else {
            const data = await exportResponse.json();
            alert(data.message || 'Успешно');
        }
    } catch (error) {
        alert('Ошибка: ' + error.message);
    }
});

// Инициализация при загрузке страницы
document.addEventListener('DOMContentLoaded', () => {
    loadTemplates();
    // Если нужно, можно также загрузить сохранённый ранее documentId из localStorage
    // currentDocumentId = localStorage.getItem('documentId');
});