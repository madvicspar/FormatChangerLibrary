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

// ========== Система оценивания ==========

const SCORING_TYPES = [
    { id: 'normal',  label: 'Основной текст' },
    { id: 'heading', label: 'Заголовки' },
    { id: 'list',    label: 'Списки' },
    { id: 'image',   label: 'Изображения' },
    { id: 'table',   label: 'Таблицы' },
];

const SCORING_RULES = {
    normal: [
        { id: 'font',    label: 'Шрифт',               w: 20 },
        { id: 'size',    label: 'Размер шрифта',        w: 20 },
        { id: 'align',   label: 'Выравнивание',         w: 15 },
        { id: 'spacing', label: 'Межстрочный интервал', w: 20 },
        { id: 'indent',  label: 'Отступ первой строки', w: 15 },
        { id: 'margins', label: 'Боковые отступы',      w: 10 },
    ],
    heading: [
        { id: 'font',    label: 'Шрифт',                  w: 20 },
        { id: 'size',    label: 'Размер шрифта',           w: 25 },
        { id: 'bold',    label: 'Полужирное начертание',   w: 20 },
        { id: 'align',   label: 'Выравнивание',            w: 15 },
        { id: 'newpage', label: 'Начало с новой страницы', w: 20 },
    ],
    list: [
        { id: 'font',   label: 'Шрифт',              w: 20 },
        { id: 'marker', label: 'Маркер',              w: 30 },
        { id: 'end',    label: 'Знак конца элемента', w: 30 },
        { id: 'indent', label: 'Отступ',              w: 20 },
    ],
    image: [
        { id: 'align',   label: 'Выравнивание',    w: 30 },
        { id: 'caption', label: 'Наличие подписи', w: 40 },
        { id: 'spacing', label: 'Интервалы',       w: 30 },
    ],
    table: [
        { id: 'caption', label: 'Наличие подписи',    w: 35 },
        { id: 'header',  label: 'Строка заголовка',   w: 25 },
        { id: 'spacing', label: 'Интервалы',          w: 20 },
        { id: 'align',   label: 'Выравнивание ячеек', w: 20 },
    ],
};

let scoringTypeWeights = {};
let scoringRuleWeights = {};
let scoringActiveRuleType = 'normal';

function buildDefaultRuleWeights() {
    const w = {};
    for (const [tid, rules] of Object.entries(SCORING_RULES)) {
        w[tid] = Object.fromEntries(rules.map(r => [r.id, r.w]));
    }
    return w;
}

function openNewScoringModal() {
    loadScoringContent(0);
}

function openEditSelectedScoringModal() {
    const id = parseInt(document.getElementById('selectedScoringId').value);
    if (id > 0) loadScoringContent(id);
}

function loadScoringContent(id) {
    const container = document.getElementById('scoring-modal-content');
    container.innerHTML = '<div class="p-5 text-center"><div class="spinner-border text-primary" role="status"></div></div>';

    const modalEl = document.getElementById('scoringSystemModal');
    bootstrap.Modal.getOrCreateInstance(modalEl).show();

    fetch(`/Home/GetScoringForEdit?id=${id}`)
        .then(r => {
            if (!r.ok) throw new Error(r.status);
            return r.text();
        })
        .then(html => {
            container.innerHTML = html;
            initAfterScoringModalLoad();
        })
        .catch(() => {
            container.innerHTML = '<div class="p-4 text-danger">Ошибка загрузки формы.</div>';
        });
}

function initAfterScoringModalLoad() {
    const readInt = id => {
        const el = document.getElementById(id);
        return el ? (parseInt(el.value) || 0) : 0;
    };

    scoringTypeWeights = {
        normal:  readInt('scoring-weight-normal'),
        heading: readInt('scoring-weight-heading'),
        list:    readInt('scoring-weight-list'),
        image:   readInt('scoring-weight-image'),
        table:   readInt('scoring-weight-table'),
    };

    const ruleJsonEl = document.getElementById('scoring-rule-weights-json');
    try {
        scoringRuleWeights = ruleJsonEl?.value ? JSON.parse(ruleJsonEl.value) : buildDefaultRuleWeights();
    } catch {
        scoringRuleWeights = buildDefaultRuleWeights();
    }

    scoringActiveRuleType = 'normal';
    renderScoringTypeGrid();
    renderScoringRuleTypeTabs();
    renderScoringRulesTable();

    const form = document.getElementById('scoring-save-form');
    if (form) form.addEventListener('submit', handleScoringSave);

    const previewTabBtn = document.querySelector('#scoring-nav-tabs [data-bs-target="#scoring-tab-preview"]');
    if (previewTabBtn) previewTabBtn.addEventListener('shown.bs.tab', renderScoringPreview);

    const rulesTabBtn = document.querySelector('#scoring-nav-tabs [data-bs-target="#scoring-tab-rules"]');
    if (rulesTabBtn) {
        rulesTabBtn.addEventListener('shown.bs.tab', function () {
            renderScoringRuleTypeTabs();
            renderScoringRulesTable();
        });
    }
}

function handleScoringSave(e) {
    e.preventDefault();
    const form = e.currentTarget;

    const setVal = (id, val) => { const el = document.getElementById(id); if (el) el.value = val; };
    setVal('scoring-weight-normal',    scoringTypeWeights.normal);
    setVal('scoring-weight-heading',   scoringTypeWeights.heading);
    setVal('scoring-weight-list',      scoringTypeWeights.list);
    setVal('scoring-weight-image',     scoringTypeWeights.image);
    setVal('scoring-weight-table',     scoringTypeWeights.table);
    setVal('scoring-rule-weights-json', JSON.stringify(scoringRuleWeights));

    const data = new FormData(form);
    fetch('/Home/SaveScoring', { method: 'POST', body: data })
        .then(r => r.json())
        .then(result => {
            if (result.success) {
                bootstrap.Modal.getInstance(document.getElementById('scoringSystemModal')).hide();
                window.location.reload();
            } else {
                alert('Ошибка сохранения:\n' + (result.errors?.join('\n') || 'Неизвестная ошибка'));
            }
        })
        .catch(() => alert('Ошибка при сохранении.'));
}

function deleteScoringFromModal(id) {
    if (!confirm('Удалить систему оценивания?')) return;

    const tokenInput = document.querySelector('#csrf-form [name="__RequestVerificationToken"]');
    const data = new FormData();
    data.append('id', id);
    if (tokenInput) data.append('__RequestVerificationToken', tokenInput.value);

    fetch('/Home/DeleteScoring', { method: 'POST', body: data })
        .then(r => r.json())
        .then(result => {
            if (result.success) {
                bootstrap.Modal.getInstance(document.getElementById('scoringSystemModal')).hide();
                window.location.reload();
            }
        })
        .catch(() => alert('Ошибка при удалении.'));
}

function scoringTotalTypeWeight() {
    return Object.values(scoringTypeWeights).reduce((a, b) => a + b, 0);
}

function scoringRuleSum(tid) {
    return Object.values(scoringRuleWeights[tid] ?? {}).reduce((a, b) => a + b, 0);
}

function updateScoringTotalBar() {
    const total = scoringTotalTypeWeight();
    const valEl = document.getElementById('scoring-total-val');
    if (valEl) {
        valEl.textContent = total;
        valEl.className = 'scoring-total-val ' + (total === 100 ? 'ok' : 'warn');
    }
}

function renderScoringTypeGrid() {
    const grid = document.getElementById('scoring-type-grid');
    if (!grid) return;
    grid.innerHTML = SCORING_TYPES.map(t => {
        const w = scoringTypeWeights[t.id] ?? 0;
        return `<div class="scoring-type-card">
            <div class="scoring-type-card-header">
                <span class="scoring-type-name">${t.label}</span>
                <span class="scoring-type-val" id="scoring-val-${t.id}">${w}</span>
            </div>
            <div class="scoring-weight-bar">
                <div class="scoring-weight-bar-fill" id="scoring-fill-${t.id}" style="width:${Math.round(w / 60 * 100)}%"></div>
            </div>
            <input type="range" class="form-range scoring-type-range"
                min="0" max="60" step="1" value="${w}" data-tid="${t.id}"
                style="accent-color:#1980e6" />
        </div>`;
    }).join('');
    updateScoringTotalBar();
    grid.querySelectorAll('.scoring-type-range').forEach(inp => {
        inp.addEventListener('input', e => {
            const tid = e.target.dataset.tid;
            scoringTypeWeights[tid] = parseInt(e.target.value);
            const valEl = document.getElementById('scoring-val-' + tid);
            const fillEl = document.getElementById('scoring-fill-' + tid);
            if (valEl) valEl.textContent = scoringTypeWeights[tid];
            if (fillEl) fillEl.style.width = Math.round(scoringTypeWeights[tid] / 60 * 100) + '%';
            updateScoringTotalBar();
        });
    });
}

function renderScoringRuleTypeTabs() {
    const el = document.getElementById('scoring-rule-type-tabs');
    if (!el) return;
    el.innerHTML = SCORING_TYPES.map(t => {
        const active = t.id === scoringActiveRuleType;
        return `<button class="btn btn-sm ${active ? 'btn-primary' : 'btn-outline-secondary'}" data-stid="${t.id}">${t.label}</button>`;
    }).join('');
    el.querySelectorAll('button').forEach(b => {
        b.addEventListener('click', e => {
            scoringActiveRuleType = e.currentTarget.dataset.stid;
            renderScoringRuleTypeTabs();
            renderScoringRulesTable();
        });
    });
}

function renderScoringRulesTable() {
    const tbody = document.getElementById('scoring-rules-body');
    if (!tbody) return;
    const tid = scoringActiveRuleType;
    const sum = scoringRuleSum(tid);
    tbody.innerHTML = SCORING_RULES[tid].map(r => {
        const w = scoringRuleWeights[tid]?.[r.id] ?? r.w;
        const pct = sum > 0 ? Math.round(w / sum * 100) : 0;
        return `<tr>
            <td class="align-middle">${r.label}</td>
            <td style="width:120px" class="align-middle">
                <input type="range" class="form-range scoring-rule-range"
                    min="0" max="50" step="5" value="${w}"
                    data-stid="${tid}" data-srid="${r.id}"
                    style="accent-color:#1980e6" />
            </td>
            <td style="width:56px;text-align:right" class="align-middle">
                <span class="scoring-rule-pct" id="scoring-rpct-${tid}-${r.id}">${pct}%</span>
            </td>
        </tr>`;
    }).join('');
    tbody.querySelectorAll('.scoring-rule-range').forEach(inp => {
        inp.addEventListener('input', e => {
            const { stid, srid } = e.target.dataset;
            scoringRuleWeights[stid][srid] = parseInt(e.target.value);
            const newSum = scoringRuleSum(stid);
            SCORING_RULES[stid].forEach(r => {
                const pctEl = document.getElementById('scoring-rpct-' + stid + '-' + r.id);
                if (pctEl) pctEl.textContent = (newSum > 0 ? Math.round(scoringRuleWeights[stid][r.id] / newSum * 100) : 0) + '%';
            });
        });
    });
}

function computeScoringWeights() {
    const result = [];
    const totalTW = scoringTotalTypeWeight();
    for (const t of SCORING_TYPES) {
        const tw = totalTW > 0 ? scoringTypeWeights[t.id] / totalTW : 0;
        const rs = scoringRuleSum(t.id);
        for (const r of SCORING_RULES[t.id]) {
            const rw = rs > 0 ? (scoringRuleWeights[t.id]?.[r.id] ?? r.w) / rs : 0;
            result.push({ type: t.label, rule: r.label, wr: Math.round(tw * rw * 100 * 100) / 100, tid: t.id });
        }
    }
    return result;
}

function renderScoringPreview() {
    const tbody = document.getElementById('scoring-preview-body');
    if (!tbody) return;
    const weights = computeScoringWeights();
    let lastType = '';
    tbody.innerHTML = weights.map(w => {
        const typeCell = w.type !== lastType
            ? `<td rowspan="${SCORING_RULES[w.tid].length}" style="vertical-align:top;font-weight:500;padding-top:10px">${w.type}</td>`
            : '';
        lastType = w.type;
        return `<tr>${typeCell}<td>${w.rule}</td><td style="text-align:right;font-variant-numeric:tabular-nums;font-weight:500">${w.wr.toFixed(2)}</td></tr>`;
    }).join('');
    const totalWr = weights.reduce((a, b) => a + b.wr, 0);
    const scoreEl = document.getElementById('scoring-preview-score');
    if (scoreEl) scoreEl.textContent = Math.round(totalWr) + ' / 100';
}

document.addEventListener('DOMContentLoaded', function () {
    const scoringSelect = document.getElementById('scoringSelect');
    if (scoringSelect) {
        scoringSelect.addEventListener('change', function () {
            document.getElementById('selectedScoringId').value = this.value;
        });
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