using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using RegistryAPI.Data;

namespace RegistryAPI.Pages.Document
{
    public class DownloadModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly FileExtensionContentTypeProvider _contentTypeProvider = new FileExtensionContentTypeProvider();

        public DownloadModel(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync(int id, bool inline = false)
        {
            var doc = await _context.Documents.Include(d => d.File).FirstOrDefaultAsync(d => d.DocumentId == id);
            if (doc == null) return NotFound();

            // Prefer DB bytes if present
            if (doc.ScanFileData != null && doc.ScanFileData.Length > 0)
            {
                var fileName = string.IsNullOrEmpty(doc.DocumentName) ? $"document_{doc.DocumentId}" : doc.DocumentName;

                // Ensure filename has an extension. If not, try to find a physical file or infer from content type.
                var ext = Path.GetExtension(fileName);
                if (string.IsNullOrEmpty(ext))
                {
                    // try to find a physical file to get original extension
                    try
                    {
                        var staffId = doc.File?.StaffId ?? "unassigned";
                        var uploadsRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", staffId);
                        if (Directory.Exists(uploadsRoot))
                        {
                            var patt = $"{doc.DocumentId}_*";
                            var files = Directory.GetFiles(uploadsRoot, patt);
                            if (files.Length > 0)
                            {
                                var phys = files[0];
                                var physName = Path.GetFileName(phys);
                                var idx = physName.IndexOf('_');
                                if (idx >= 0 && idx + 1 < physName.Length)
                                {
                                    var original = physName.Substring(idx + 1);
                                    var origExt = Path.GetExtension(original);
                                    if (!string.IsNullOrEmpty(origExt))
                                    {
                                        ext = origExt;
                                        fileName = fileName + ext;
                                    }
                                }
                            }
                        }
                    }
                    catch { }
                }

                if (!_contentTypeProvider.TryGetContentType(fileName, out var contentType))
                {
                    // fallback by common mapping if provider failed
                    contentType = doc.DocumentName != null && doc.DocumentName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase)
                        ? "application/pdf"
                        : "application/octet-stream";
                }

                // If attachment (download), let framework set filename header via File(..., fileDownloadName)
                if (!inline)
                {
                    return File(doc.ScanFileData, contentType, fileName);
                }

                // For inline, set Content-Disposition explicitly and return bytes
                Response.Headers["Content-Disposition"] = $"inline; filename=\"{fileName}\"";
                return File(doc.ScanFileData, contentType);
            }

            // Otherwise attempt to serve physical file if exists
            try
            {
                var staffId = doc.File?.StaffId ?? "unassigned";
                var uploadsRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", staffId);
                // look for file that starts with DocumentId_
                if (Directory.Exists(uploadsRoot))
                {
                    var patt = $"{doc.DocumentId}_*";
                    var files = Directory.GetFiles(uploadsRoot, patt);
                    if (files.Length > 0)
                    {
                        var path = files[0];
                        if (!_contentTypeProvider.TryGetContentType(path, out var physicalContentType))
                        {
                            physicalContentType = "application/octet-stream";
                        }
                        var downloadName = Path.GetFileName(path);
                        if (!inline)
                        {
                            return PhysicalFile(path, physicalContentType, downloadName);
                        }
                        Response.Headers["Content-Disposition"] = $"inline; filename=\"{downloadName}\"";
                        var fs = System.IO.File.OpenRead(path);
                        return File(fs, physicalContentType);
                    }
                }
            }
            catch
            {
                // ignore and fall through to 404
            }

            return NotFound();
        }
    }
}
