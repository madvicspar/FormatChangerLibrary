// Функция для отображения формы загрузки
function showUploadForm() {
    var form = document.getElementById('uploadForm');
    form.style.display = 'block';
}

function startFormattingProcess() {
    var selectedTemplateId = document.getElementById('templateSelect').value;
    var selectedActionId = document.getElementById('actionSelect').value;
    const paragraphData = getParagraphTypes();

    fetch(`/Home/Export?templateId=${selectedTemplateId}&actionId=${selectedActionId}`, {
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

function getParagraphTypes() {
    const paragraphs = document.querySelectorAll('.text-block');
    const paragraphData = [];

    paragraphs.forEach((paragraph) => {
        paragraphData.push(paragraph.dataset.type); // Добавляем тип абзаца
    });

    return paragraphData;
}

document.addEventListener('DOMContentLoaded', function () {
    let currentIndex = 0; // Индекс активного параграфа
    const paragraphs = document.querySelectorAll('#paragraphsContainer .text-block');
    const typeOptions = document.querySelectorAll('.type-option');

    // Отображаем первый параграф как активный
    if (paragraphs.length > 0) {
        paragraphs[currentIndex].classList.add('highlighted');
        updateActiveTypeButton(paragraphs[currentIndex]);
    }

    // Функция для обновления активного параграфа
    function updateActiveParagraph() {
        paragraphs.forEach((p, index) => {
            p.classList.remove('highlighted'); // Убираем активный класс у всех
            if (index === currentIndex) {
                p.classList.add('highlighted'); // Добавляем активный класс к текущему параграфу
                updateActiveTypeButton(p); // Синхронизируем тип с кнопкой
            }
        });
    }

    // Обработчик нажатия на кнопку "Предыдущий абзац"
    document.querySelector('.prev-btn').addEventListener('click', function () {
        if (currentIndex > 0) {
            currentIndex--;
            updateActiveParagraph();
        }
    });

    // Обработчик нажатия на кнопку "Следующий абзац"
    document.querySelector('.next-btn').addEventListener('click', function () {
        if (currentIndex < paragraphs.length - 1) {
            currentIndex++;
            updateActiveParagraph();
        }
    });


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

    function updateActiveTypeButton(paragraph) {
        const type = paragraph.dataset.type;

        typeOptions.forEach(option => {
            option.classList.remove('active');
            if (option.getAttribute('data-type') === type) {
                option.classList.add('active');
            }
        });
    }
});