namespace Bcp.Application.DTOs;

/// <summary>
/// Payload sent from the worker to the API to indicate a file has been processed.
/// </summary>
/// <param name="FileId">Identifier of the file created in the database.</param>
/// <param name="FileName">Name of the file (filename.ext).</param>
public record FileProcessed(int FileId, string FileName);