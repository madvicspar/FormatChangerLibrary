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
        paragraphs[currentIndex].classList.add('highlighted');
        updateActiveTypeButton(paragraphs[currentIndex]);
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
}

// Сделать абзац активным
function updateActiveParagraph() {
    paragraphs.forEach((p, index) => {
        p.classList.remove('highlighted');
        if (index === currentIndex) {
            p.classList.add('highlighted');
            updateActiveTypeButton(p);
        }
    });
}

// Обеспечить выбор типа абзаца
function setupTypeSelection() {
    typeOptions.forEach(option => {
        option.addEventListener('click', function () {
            const highlightedParagraph = document.querySelector('.text-block.highlighted');
            if (highlightedParagraph) {
                const newType = option.getAttribute('data-type');
                highlightedParagraph.dataset.type = newType; // Обновляем тип абзаца
                updateActiveTypeButton(highlightedParagraph); // Синхронизируем кнопку
            }
        });
    });
}

// Обновить выбранный тип абзаца
function updateActiveTypeButton(paragraph) {
    const type = paragraph.dataset.type;

    typeOptions.forEach(option => {
        option.classList.remove('active');
        if (option.getAttribute('data-type') === type) {
            option.classList.add('active');
        }
    });
}