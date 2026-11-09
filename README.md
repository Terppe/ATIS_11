# ATIS - V7.8

## PDF export

List and record exports use the singleton `IPdfExportService` / `PdfExportService`
registered in `App.ConfigureServices`. The service creates the document, opens a
window-initialized PDF save picker in Documents, sanitizes the suggested filename,
and reports unavailable documents, export errors, or the saved file path through
the localized dialog service. Cancelling the picker does not generate a PDF or
show a success message. Export operations are serialized to avoid overlapping
pickers and dialogs.

All 22 taxonomy detail pages invoke their ViewModel's `ExportPdfCommand`; the
Disease, Food, and Miscellaneous detail pages also use the service. Records are
no longer saved automatically to the desktop.

Search list export supports all 24 search tables, including Disease, Food, and
Miscellaneous. It preserves the visible result order and captures the selected
table and items before waiting for another export, so subsequent tab changes do
not affect the queued document. Search versioning, grouped tabs, and the Genus
default from `copilot/search-pdf-country-updates` are retained.