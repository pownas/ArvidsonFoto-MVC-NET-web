# OpenTelemetry tracing

## Local collection

The web app uses the OpenTelemetry setup in `ArvidsonFoto.ServiceDefaults`. ASP.NET Core creates the incoming HTTP server spans, and the configured `HttpClient` instrumentation creates client spans and propagates W3C Trace Context. Health-check requests are excluded. The `ArvidsonFoto` activity source adds spans for selected application operations.

To inspect traces locally, start the Aspire AppHost:

```bash
dotnet run --project ArvidsonFoto.AppHost
```

Open the Aspire dashboard and select **Traces**. For a separately hosted app or another local collector, set `OTEL_EXPORTER_OTLP_ENDPOINT` to its OTLP endpoint. The existing configuration enables the OTLP exporter when this variable is set. Do not put credentials or personal data in exporter headers or configuration committed to the repository.

The app exports through OTLP rather than depending on a specific storage backend. A collector can route the same telemetry to a local dashboard now and to a centralized tracing backend later.

## Correlation and span details

ASP.NET Core reads the incoming W3C `traceparent` and `tracestate` headers. Outgoing `HttpClient` requests inherit the current trace context automatically; do not create a parallel request ID or manually rewrite trace headers. SMTP has no standard W3C header propagation, so its operation appears as a child span in the current trace instead.

For relational database providers, EF Core instrumentation emits a child span for each database command. These spans expose sanitized SQL text or query summaries (depending on the selected database semantic-convention version), operation type, provider, duration, and errors, so a trace shows the number and ordering of SQL commands under the application operation that triggered them. EF Core instrumentation currently supports relational providers; the in-memory provider used by tests does not emit SQL-command spans.

| Span name | Operation | Attributes |
| --- | --- | --- |
| `category.list` | List categories, including cache lookup and retrieval | `arvidsonfoto.cache.hit`, `arvidsonfoto.result.count` |
| `image.list` | List images | `arvidsonfoto.result.count` |
| `contact.save` | Persist a contact submission | None |
| `smtp.send` | Send contact email | None |
| EF Core database command | One relational database command | Sanitized SQL text/query summary, database system and operation |

Errors in these application spans are marked with error status and `error.type`; exception messages and bodies are not copied to span attributes. EF Core instrumentation sanitizes SQL literal values, but SQL identifiers may still appear in query text. Query parameter values are not exported by default; do not enable `OTEL_DOTNET_EXPERIMENTAL_EFCORE_ENABLE_TRACE_DB_QUERY_PARAMETERS` in environments where query values could contain personal data. ASP.NET Core and HTTP client instrumentation report HTTP status and transport failures. Span attributes must never contain names, email addresses, form contents, access tokens, passwords, or other secrets.

## Troubleshooting

Find the incoming server span by its route and status, then follow child spans to identify the application operation that was slow or failed. Expand its EF Core child spans to inspect sanitized query summaries and count repeated or sequential database commands. Use the shared trace ID to correlate spans across instrumented HTTP dependencies. If no traces appear in the dashboard, confirm that the app is running with the Aspire AppHost or that `OTEL_EXPORTER_OTLP_ENDPOINT` points to a reachable OTLP collector.

This approach follows [DIGG's guidance on traceability and correlation](https://www.dataportal.se/rest-api-profil/sparbarhet-och-korrelation) and the [W3C Trace Context standard](https://www.w3.org/TR/trace-context/).
