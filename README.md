# ATIS - V7.8

## PDF export

List and record exports use the singleton `IPdfExportService` / `PdfExportService`
registered in `App.ConfigureServices`. The service creates the document, opens a
window-initialized PDF save picker in Documents, sanitizes the suggested filename,
and reports unavailable documents, export errors, or the saved file path through
the localized dialog service. Cancelling the picker does not generate a PDF or
show a success message. Export operations are serialized to avoid overlapping
pickers and dialogs.

All 22 search detail pages invoke their ViewModel's `ExportPdfCommand`; records
are no longer saved automatically to the desktop. Search list export continues
to support the visible Class results only and uses the same service.