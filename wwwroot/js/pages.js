// Consolidated page scripts extracted from Razor pages

// _Layout sidebar toggle
document.addEventListener('DOMContentLoaded', function () {
    (function () {
        var sidebar = document.getElementById('sidebar');
        var toggle = document.getElementById('sidebarToggle');
        var overlay = document.getElementById('mobileOverlay');
        if (toggle && sidebar) toggle.onclick = function () { sidebar.classList.toggle('collapsed'); };
        if (overlay) overlay.onclick = function () { sidebar && sidebar.classList.remove('show'); overlay.classList.remove('show'); };
    })();

    // Initialize Bootstrap tooltips universally if present
    try {
        var tooltipTriggerList = document.querySelectorAll('[data-bs-toggle="tooltip"]');
        tooltipTriggerList.forEach(function (tooltipTriggerEl) {
            if (window.bootstrap && bootstrap.Tooltip) new bootstrap.Tooltip(tooltipTriggerEl);
        });
    } catch (e) { console.warn('Tooltip init failed', e); }
});

// Termination delete confirmation (safe-guarded by element existence checks)
(function () {
    function initTerminationDelete() {
        var confirmCheckbox = document.getElementById('confirmDelete');
        var deleteBtn = document.getElementById('deleteBtn');
        var deleteForm = document.getElementById('deleteForm');

        if (confirmCheckbox && deleteBtn) {
            confirmCheckbox.addEventListener('change', function () {
                deleteBtn.disabled = !this.checked;
            });
        }

        if (deleteForm) {
            deleteForm.addEventListener('submit', function (e) {
                if (confirmCheckbox && !confirmCheckbox.checked) {
                    e.preventDefault();
                    return;
                }

                var isConfirmed = confirm('⚠️ FINAL WARNING: This will permanently delete the termination record.\n\nAre you absolutely sure?');
                if (!isConfirmed) e.preventDefault();
            });
        }

        if (deleteBtn) {
            deleteBtn.addEventListener('mouseenter', function () {
                if (!this.disabled) this.innerHTML = '<i class="fas fa-skull-crossbones me-2"></i>Delete Permanently';
            });
            deleteBtn.addEventListener('mouseleave', function () {
                if (!this.disabled) this.innerHTML = '<i class="fas fa-trash-alt me-2"></i>Delete Permanently';
            });
        }
    }

    if (document.readyState === 'loading') document.addEventListener('DOMContentLoaded', initTerminationDelete);
    else initTerminationDelete();
})();

(function () {
    function initDepartmentEdit() {
        try {
            // tooltip init (safe)
            var tooltipTriggerList = document.querySelectorAll('[data-bs-toggle="tooltip"]');
            tooltipTriggerList.forEach(function (tooltipTriggerEl) { if (window.bootstrap && bootstrap.Tooltip) new bootstrap.Tooltip(tooltipTriggerEl); });
        } catch (e) { console.warn('Tooltip init failed', e); }

        // Character count
        var deptCodeInput = document.querySelector('input[name="Department.DeptCode"]');
        var deptNameInput = document.querySelector('input[name="Department.DeptName"]');
        var charCountCode = document.getElementById('charCountCode');
        var charCountName = document.getElementById('charCountName');

        function updateCharCount(input, countElement, maxLength) {
            if (!input || !countElement) return;
            var length = input.value.length;
            countElement.textContent = length;
            if (length > maxLength * 0.8) { countElement.classList.remove('text-success', 'text-warning'); countElement.classList.add('text-danger'); }
            else if (length > maxLength * 0.6) { countElement.classList.remove('text-success', 'text-danger'); countElement.classList.add('text-warning'); }
            else { countElement.classList.remove('text-warning', 'text-danger'); countElement.classList.add('text-success'); }
        }

        if (deptCodeInput && charCountCode) { deptCodeInput.addEventListener('input', function () { updateCharCount(this, charCountCode, 20); }); updateCharCount(deptCodeInput, charCountCode, 20); }
        if (deptNameInput && charCountName) { deptNameInput.addEventListener('input', function () { updateCharCount(this, charCountName, 100); }); updateCharCount(deptNameInput, charCountName, 100); }

        // Form validation simple
        var editForm = document.getElementById('editForm');
        if (editForm) {
            editForm.addEventListener('submit', function (e) {
                var inputs = this.querySelectorAll('input[required]');
                var isValid = true;
                inputs.forEach(function (input) {
                    if (!input.value.trim()) {
                        isValid = false; input.classList.add('is-invalid');
                        var errorSpan = input.parentElement.nextElementSibling?.nextElementSibling;
                        if (errorSpan && errorSpan.classList.contains('text-danger')) errorSpan.textContent = 'This field is required';
                    } else input.classList.remove('is-invalid');
                });
                if (!isValid) { e.preventDefault(); alert('Please fill in all required fields'); }
            });
        }

        // input focus effects
        var formInputs = document.querySelectorAll('.form-control');
        formInputs.forEach(function (input) {
            input.addEventListener('focus', function () { this.parentElement.style.boxShadow = '0 0 0 0.25rem rgba(0, 102, 204, 0.15)'; });
            input.addEventListener('blur', function () { this.parentElement.style.boxShadow = 'none'; });
        });

        // preview button guard
        var previewBtn = document.querySelector('a[href*="Details"]');
        if (previewBtn) {
            previewBtn.addEventListener('click', function (e) {
                var hasChanges = (deptCodeInput && deptCodeInput.value) || (deptNameInput && deptNameInput.value);
                if (hasChanges && !confirm('You have unsaved changes. Do you want to preview without saving?')) e.preventDefault();
            });
        }
    }

    if (document.readyState === 'loading') document.addEventListener('DOMContentLoaded', initDepartmentEdit);
    else initDepartmentEdit();
})();

// Page-specific scripts consolidated here. Additional scripts extracted from other pages should
// be appended here and guarded with element existence checks to avoid errors on pages
// where elements don't exist.

// Document Index: tooltip init (guarded)
(function () {
    function initDocumentIndex() {
        try {
            var tooltipTriggerList = document.querySelectorAll('[data-bs-toggle="tooltip"]');
            tooltipTriggerList.forEach(function (tooltipTriggerEl) {
                try { if (window.bootstrap && bootstrap.Tooltip) new bootstrap.Tooltip(tooltipTriggerEl); } catch (e) { console.warn('Tooltip init failed', e); }
            });
        } catch (e) { console.warn('Document index tooltip init failed', e); }
    }

    if (document.readyState === 'loading') document.addEventListener('DOMContentLoaded', initDocumentIndex);
    else initDocumentIndex();
})();

// Document Create: upload file size validation (reads data attributes)
(function () {
    function initDocumentCreate() {
        var fileInput = document.getElementById('Upload');
        if (!fileInput) return;

        var maxBytes = parseInt(fileInput.dataset.maxBytes || '0', 10);
        var maxMB = fileInput.dataset.maxMb || '';

        function bytesToMB(b) { return (b / 1024 / 1024).toFixed(2); }

        fileInput.addEventListener('change', function () {
            var f = fileInput.files && fileInput.files[0];
            var msgElId = 'upload-size-msg';
            var msgEl = document.getElementById(msgElId);
            if (!msgEl) {
                msgEl = document.createElement('div');
                msgEl.id = msgElId;
                msgEl.className = 'mt-2';
                fileInput.parentNode.parentNode.appendChild(msgEl);
            }
            var form = fileInput.closest('form');
            if (!f) {
                msgEl.textContent = '';
                msgEl.className = 'mt-2';
                if (form) form.querySelector('button[type=submit]').disabled = false;
                return;
            }
            if (maxBytes && f.size > maxBytes) {
                msgEl.textContent = 'File too large (' + bytesToMB(f.size) + ' MB). Maximum is ' + maxMB + ' MB.';
                msgEl.className = 'text-danger mt-2';
                if (form) form.querySelector('button[type=submit]').disabled = true;
            } else {
                msgEl.textContent = 'File size: ' + bytesToMB(f.size) + ' MB';
                msgEl.className = 'text-success mt-2';
                if (form) form.querySelector('button[type=submit]').disabled = false;
            }
        });
    }

    if (document.readyState === 'loading') document.addEventListener('DOMContentLoaded', initDocumentCreate);
    else initDocumentCreate();
})();

// End of consolidated page scripts

(function () {
    function initDepartmentIndex() {
        try {
            var dropdownElements = document.querySelectorAll('[data-bs-toggle="dropdown"]');
            dropdownElements.forEach(function (dropdownElement) {
                try { if (window.bootstrap && bootstrap.Dropdown) new bootstrap.Dropdown(dropdownElement); } catch (e) { console.warn('Dropdown init failed', e); }
            });
        } catch (e) { console.warn('Dropdown setup failed', e); }

        // Row selection / bulk actions
        var selectAllCheckbox = document.getElementById('selectAll');
        var departmentCheckboxes = document.querySelectorAll('.department-checkbox');
        var bulkDeleteBtn = document.getElementById('bulkDeleteBtn');
        var bulkExportBtn = document.getElementById('bulkExportBtn');
        var departmentRows = document.querySelectorAll('.department-row');

        function updateSelectAllCheckbox() {
            if (!selectAllCheckbox) return;
            var allChecked = Array.from(departmentCheckboxes).every(cb => cb.checked);
            var someChecked = Array.from(departmentCheckboxes).some(cb => cb.checked);
            selectAllCheckbox.checked = allChecked;
            selectAllCheckbox.indeterminate = someChecked && !allChecked;
        }

        function updateBulkButtons() {
            var selectedCount = Array.from(departmentCheckboxes).filter(cb => cb.checked).length;
            if (bulkDeleteBtn) {
                bulkDeleteBtn.disabled = selectedCount === 0;
                bulkDeleteBtn.innerHTML = selectedCount > 0
                    ? `<i class="fas fa-trash me-1"></i>Delete (${selectedCount})`
                    : `<i class="fas fa-trash me-1"></i>Delete Selected`;
            }
            if (bulkExportBtn) {
                bulkExportBtn.disabled = selectedCount === 0;
                bulkExportBtn.innerHTML = selectedCount > 0
                    ? `<i class="fas fa-download me-1"></i>Export (${selectedCount})`
                    : `<i class="fas fa-download me-1"></i>Export Selected`;
            }
        }

        if (selectAllCheckbox) {
            selectAllCheckbox.addEventListener('change', function () {
                var isChecked = this.checked;
                departmentCheckboxes.forEach(cb => cb.checked = isChecked);
                updateBulkButtons();
            });
        }

        departmentCheckboxes.forEach(cb => cb.addEventListener('change', function () { updateSelectAllCheckbox(); updateBulkButtons(); }));

        departmentRows.forEach(row => row.addEventListener('click', function (e) {
            if (!e.target.closest('a') && !e.target.closest('input[type="checkbox"]')) {
                var checkbox = this.querySelector('.department-checkbox');
                if (checkbox) { checkbox.checked = !checkbox.checked; checkbox.dispatchEvent(new Event('change')); }
            }
        }));

        if (bulkDeleteBtn) bulkDeleteBtn.addEventListener('click', function () {
            var selectedIds = Array.from(departmentCheckboxes).filter(cb => cb.checked).map(cb => cb.value);
            if (selectedIds.length && confirm(`Are you sure you want to delete ${selectedIds.length} selected department(s)?\n\nThis action cannot be undone.`)) {
                console.log('Bulk deleting departments:', selectedIds);
                alert(`${selectedIds.length} department(s) marked for deletion.`);
            }
        });

        if (bulkExportBtn) bulkExportBtn.addEventListener('click', function () {
            var selectedIds = Array.from(departmentCheckboxes).filter(cb => cb.checked).map(cb => cb.value);
            if (selectedIds.length) { console.log('Exporting departments:', selectedIds); alert(`Exporting ${selectedIds.length} department(s)...`); }
        });

        var searchInput = document.querySelector('input[name="search"]');
        if (searchInput) {
            searchInput.addEventListener('focus', function () { this.parentElement.style.boxShadow = '0 0 0 0.25rem rgba(0, 102, 204, 0.15)'; });
            searchInput.addEventListener('blur', function () { this.parentElement.style.boxShadow = 'none'; });
        }

        departmentRows.forEach(row => {
            row.addEventListener('mouseenter', function () { var checkbox = this.querySelector('.department-checkbox'); if (checkbox && !checkbox.checked) this.style.backgroundColor = 'rgba(0, 102, 204, 0.02)'; });
            row.addEventListener('mouseleave', function () { var checkbox = this.querySelector('.department-checkbox'); if (checkbox && !checkbox.checked) this.style.backgroundColor = ''; });
        });

        updateBulkButtons();
    }

    if (document.readyState === 'loading') document.addEventListener('DOMContentLoaded', initDepartmentIndex);
    else initDepartmentIndex();
})();

// Document delete confirmation
(function () {
    function initDocumentDelete() {
        var confirmCheckbox = document.getElementById('confirmDelete');
        var deleteBtn = document.getElementById('deleteBtn');
        var deleteForm = document.getElementById('deleteForm');

        if (confirmCheckbox && deleteBtn) {
            confirmCheckbox.addEventListener('change', function () { deleteBtn.disabled = !this.checked; });
        }

        if (deleteForm) {
            deleteForm.addEventListener('submit', function (e) {
                if (confirmCheckbox && !confirmCheckbox.checked) { e.preventDefault(); return; }
                var isConfirmed = confirm('⚠️ FINAL WARNING: This will permanently delete the document and its associated file.\n\nAre you absolutely sure?');
                if (!isConfirmed) e.preventDefault();
            });
        }

        if (deleteBtn) {
            deleteBtn.addEventListener('mouseenter', function () { if (!this.disabled) this.innerHTML = '<i class="fas fa-skull-crossbones me-2"></i>Delete Permanently'; });
            deleteBtn.addEventListener('mouseleave', function () { if (!this.disabled) this.innerHTML = '<i class="fas fa-trash-alt me-2"></i>Delete Permanently'; });
        }
    }

    if (document.readyState === 'loading') document.addEventListener('DOMContentLoaded', initDocumentDelete);
    else initDocumentDelete();
})();
