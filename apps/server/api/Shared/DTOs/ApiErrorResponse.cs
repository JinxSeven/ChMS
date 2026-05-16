namespace api.Shared.DTOs
{
    public class ApiErrorResponse
    {
        public string? Type { get; set; }
        public required string Title { get; set; }
        public int Status { get; set; }

        /// <summary>
        /// Dictionary of field names and their error messages.
        /// </summary>
        public Dictionary<string, string[]>? Errors { get; set; }

        /// <example>
        /// Example of how this response looks in JSON:
        /// <code>
        /// {
        ///   "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
        ///   "title": "One or more validation errors occurred.",
        ///   "status": 400,
        ///   "errors": {
        ///     "Name": [
        ///       "The Name field is required.",
        ///       "The field Name must be a string or array type with a minimum length of '3'."
        ///     ],
        ///     "Email": [
        ///       "Invalid Email Address"
        ///     ]
        ///   },
        ///   "traceId": "00-33b5d01f5f220ff7f753b619d8997d19-6b6932f0334255b6-00"
        /// }
        /// </code>
        /// </example>

        public string? TraceId { get; set; }
    }
}
