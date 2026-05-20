let currentIndex = 0;
let paragraphs = [];
let typeOptions = [];

// Отобразить форму загрузки
function showUploadForm() {
    var form = document.getElementById('uploadForm');
    form.style.display = 'block';
}

// ========== CRUD шаблонов ==========

function selectTemplate(id, rowEl) {
    document.getElementById('selectedTemplateId').value = id;
    document.querySelectorAll('.template-row').forEach(r => r.classList.remove('selected'));
    rowEl.classList.add('selected');
}

function openNewTemplateModal() {
    loadModalContent(0);
}

function openEditTemplateModal(id) {
    loadModalContent(id);
}

function loadModalContent(id) {
    const container = document.getElementById('template-modal-content');
    container.innerHTML = '<div class="p-5 text-center"><div class="spinner-border text-primary" role="status"></div></div>';

    const modalEl = document.getElementById('formattingTemplateModal');
    const modal = bootstrap.Modal.getOrCreateInstance(modalEl);
    modal.show();

    fetch(`/Home/GetTemplateForEdit?id=${id}`)
        .then(r => {
            if (!r.ok) throw new Error(r.status);
            return r.text();
        })
        .then(html => {
            container.innerHTML = html;
            initAfterModalLoad();
        })
        .catch(() => {
            container.innerHTML = '<div class="p-4 text-danger">Ошибка загрузки формы.</div>';
        });
}

function initAfterModalLoad() {
    const modal = document.getElementById('formattingTemplateModal');
    updateAddButtonVisibility();
    initFormToggles(modal);
    initTooltips(modal);

    const form = document.getElementById('template-save-form');
    if (form) {
        form.addEventListener('submit', handleTemplateSave);
    }
}

function handleTemplateSave(e) {
    e.preventDefault();
    const form = e.currentTarget;
    const data = new FormData(form);

    fetch('/Home/SaveTemplate', { method: 'POST', body: data })
        .then(r => r.json())
        .then(result => {
            if (result.success) {
                bootstrap.Modal.getInstance(document.getElementById('formattingTemplateModal')).hide();
                window.location.reload();
            } else {
                const msgs = result.errors?.join('\n') || 'Неизвестная ошибка';
                alert('Ошибка сохранения:\n' + msgs);
            }
        })
        .catch(() => alert('Ошибка при сохранении шаблона.'));
}

function deleteTemplate(id) {
    const row = document.querySelector(`.template-row[data-id="${id}"]`);
    const title = row?.querySelector('.template-row-title')?.textContent?.trim() || '';
    if (!confirm(`Удалить шаблон «${title}»?`)) return;

    const tokenInput = document.querySelector('#csrf-form [name="__RequestVerificationToken"]');
    const data = new FormData();
    data.append('id', id);
    if (tokenInput) data.append('__RequestVerificationToken', tokenInput.value);

    fetch('/Home/DeleteTemplate', { method: 'POST', body: data })
        .then(r => r.json())
        .then(result => {
            if (result.success) {
                row?.remove();
                const remaining = document.querySelector('.template-row');
                const hiddenInput = document.getElementById('selectedTemplateId');
                if (remaining) {
                    hiddenInput.value = remaining.dataset.id;
                    remaining.classList.add('selected');
                } else {
                    hiddenInput.value = 0;
                }
            }
        })
        .catch(() => alert('Ошибка при удалении шаблона.'));
}

document.addEventListener('DOMContentLoaded', function () {
    const templateSelect = document.getElementById('templateSelect');
    if (templateSelect) {
        templateSelect.addEventListener('change', function () {
            document.getElementById('selectedTemplateId').value = this.value;
        });
    }
});

function openEditSelectedTemplateModal() {
    const id = parseInt(document.getElementById('selectedTemplateId').value);
    if (id > 0) openEditTemplateModal(id);
}

function deleteTemplateFromModal(id) {
    const row = document.querySelector(`.template-row[data-id="${id}"]`);
    const title = row?.querySelector('.template-row-title')?.textContent?.trim() || 'этот шаблон';
    if (!confirm(`Удалить шаблон «${title}»?`)) return;

    const tokenInput = document.querySelector('#csrf-form [name="__RequestVerificationToken"]');
    const data = new FormData();
    data.append('id', id);
    if (tokenInput) data.append('__RequestVerificationToken', tokenInput.value);

    fetch('/Home/DeleteTemplate', { method: 'POST', body: data })
        .then(r => r.json())
        .then(result => {
            if (result.success) {
                bootstrap.Modal.getInstance(document.getElementById('formattingTemplateModal')).hide();
                row?.remove();
                const hiddenInput = document.getElementById('selectedTemplateId');
                const remaining = document.querySelector('.template-row');
                if (remaining) {
                    hiddenInput.value = remaining.dataset.id;
                    remaining.classList.add('selected');
                } else {
                    hiddenInput.value = 0;
                }
            }
        })
        .catch(() => alert('Ошибка при удалении шаблона.'));
}

// Начать процесс форматирования
function startFormattingProcess() {
    var selectedTemplateId = document.getElementById('selectedTemplateId').value;
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

const MAX_HEADING_LEVELS = 3;

function updateAddButtonVisibility() {
    const btn = document.getElementById('add-heading-btn');
    if (!btn) return;
    const count = document.querySelectorAll('#headings-list .heading-card').length;
    btn.style.display = count >= MAX_HEADING_LEVELS ? 'none' : '';
}

function reindexCards() {
    const cards = document.querySelectorAll('#headings-list .heading-card');
    const lastIdx = cards.length - 1;

    cards.forEach((card, idx) => {
        const collapseId = `heading-collapse-${idx}`;

        const header = card.querySelector('.card-header');
        if (header) {
            header.setAttribute('data-bs-target', `#${collapseId}`);
        }

        const collapseDiv = card.querySelector('.collapse');
        if (collapseDiv) {
            collapseDiv.id = collapseId;
        }

        const headerSpan = card.querySelector('.card-header span');
        if (headerSpan) headerSpan.innerText = `Заголовок уровня ${idx + 1}`;

        const hiddenLevel = card.querySelector('.heading-level');
        if (hiddenLevel) hiddenLevel.value = idx + 1;

        const settingsTitle = card.querySelector('.card-body h6');
        if (settingsTitle) settingsTitle.innerText = `Настройки текста заголовка уровня ${idx + 1}`;

        card.querySelectorAll('[name]').forEach(el => {
            const oldName = el.getAttribute('name');
            const newName = oldName.replace(/HeadingLevelsEdit\[\d+\]/, `HeadingLevelsEdit[${idx}]`);
            el.setAttribute('name', newName);
        });

        card.querySelectorAll('[id]').forEach(el => {
            const oldId = el.getAttribute('id');
            const newId = oldId.replace(/HeadingLevelsEdit_\d+__/, `HeadingLevelsEdit_${idx}__`);
            el.setAttribute('id', newId);
        });

        card.querySelectorAll('[for]').forEach(el => {
            const oldFor = el.getAttribute('for');
            const newFor = oldFor.replace(/HeadingLevelsEdit_\d+__/, `HeadingLevelsEdit_${idx}__`);
            el.setAttribute('for', newFor);
        });

        const removeBtn = card.querySelector('.remove-heading');
        if (idx === lastIdx && idx > 0) {
            if (!removeBtn) {
                const btn = document.createElement('button');
                btn.type = 'button';
                btn.className = 'btn-close remove-heading';
                btn.setAttribute('aria-label', 'Удалить');
                header.appendChild(btn);
            }
        } else {
            if (removeBtn) removeBtn.remove();
        }
    });

    updateAddButtonVisibility();
}

function addHeadingCard() {
    const container = document.getElementById('headings-list');
    if (!container) return;
    if (container.querySelectorAll('.heading-card').length >= MAX_HEADING_LEVELS) return;

    const firstCard = container.querySelector('.heading-card');
    if (!firstCard) return;

    const clone = firstCard.cloneNode(true);

    clone.querySelectorAll('input, select, textarea').forEach(el => {
        if (el.type === 'checkbox' || el.type === 'radio') {
            el.checked = false;
        } else if (el.type === 'color') {
            el.value = '#000000';
        } else {
            el.value = '';
        }
    });

    const hiddenId = clone.querySelector('.heading-id');
    if (hiddenId) hiddenId.value = -Date.now();

    const collapseDiv = clone.querySelector('.collapse');
    if (collapseDiv) collapseDiv.classList.remove('show');

    container.appendChild(clone);
    reindexCards();
}

// Capture-phase handler so remove-heading fires before Bootstrap collapse
document.addEventListener('click', function (e) {
    const removeBtn = e.target.closest('.remove-heading');
    if (removeBtn) {
        e.stopPropagation();
        const card = removeBtn.closest('.heading-card');
        if (!card) return;
        if (document.querySelectorAll('#headings-list .heading-card').length <= 1) return;
        card.remove();
        reindexCards();
    }
}, true);

document.addEventListener('click', function (e) {
    if (e.target.id === 'add-heading-btn') {
        addHeadingCard();
    }
});

// ========== Авточередование маркеров нумерованного списка ==========

function updateNumberedMarkers(selectEl) {
    const card = selectEl.closest('.card, .tab-pane');
    if (!card) return;
    const isBracket = selectEl.value === '0';
    card.querySelectorAll('.numbered-marker-display').forEach(span => {
        const level = parseInt(span.dataset.level);
        if (level === 1) span.textContent = isBracket ? 'N)' : 'N.';
        else if (level === 2) span.textContent = isBracket ? 'N.' : 'N)';
        else if (level === 3) span.textContent = isBracket ? 'N)' : 'N.';
    });
}

// ========== Условные поля формы (межстрочный интервал, первая строка) ==========

function toggleLineSpacingField(select) {
    const row = select.closest('.line-spacing-row');
    if (!row) return;
    const field = row.querySelector('.line-spacing-multiplier-field');
    if (field) field.style.display = select.value === 'multiple' ? '' : 'none';
}

function toggleFirstLineField(select) {
    const wrapper = select.closest('.first-line-wrapper');
    if (!wrapper) return;
    const field = wrapper.querySelector('.first-line-value-wrapper');
    if (field) field.style.display = select.value === 'none' ? 'none' : '';
}

function initFormToggles(root) {
    const scope = root || document;
    scope.querySelectorAll('.line-spacing-type').forEach(toggleLineSpacingField);
    scope.querySelectorAll('.first-line-type').forEach(toggleFirstLineField);
}

// ========== Bootstrap Tooltips ==========

function initTooltips(root) {
    const scope = root || document;
    scope.querySelectorAll('[data-bs-toggle="tooltip"]').forEach(el => {
        if (!bootstrap.Tooltip.getInstance(el)) {
            new bootstrap.Tooltip(el);
        }
    });
}

// ========== Делегированные обработчики изменений ==========

document.addEventListener('change', function (e) {
    if (e.target.classList.contains('level1-marker-select')) {
        updateNumberedMarkers(e.target);
    }
    if (e.target.classList.contains('line-spacing-type')) {
        toggleLineSpacingField(e.target);
    }
    if (e.target.classList.contains('first-line-type')) {
        toggleFirstLineField(e.target);
    }
});

// ========== Инициализация при открытии модального окна шаблона ==========

document.addEventListener('DOMContentLoaded', function () {
    const modal = document.getElementById('formattingTemplateModal');
    if (modal) {
        modal.addEventListener('shown.bs.modal', function () {
            // initAfterModalLoad handles init once content is loaded
        });
    }

    const headingsTab = document.querySelector('button[data-bs-target="#headings"]');
    if (headingsTab) {
        headingsTab.addEventListener('shown.bs.tab', function () {
            updateAddButtonVisibility();
        });
    }
});