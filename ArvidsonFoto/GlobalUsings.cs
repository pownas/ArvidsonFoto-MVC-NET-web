// Controllers
global using ArvidsonFoto.Controllers;

// ASP.NET Core
global using Microsoft.AspNetCore.Mvc;

// Logging
global using Serilog;

// Common annotations
global using System.ComponentModel.DataAnnotations.Schema;

// Note: ArvidsonFoto.Data and ArvidsonFoto.Models are not included here
// because there are duplicate model names between ArvidsonFoto.Models and ArvidsonFoto.Core.Models
// which would cause ambiguous reference errors. See MODERNIZATION.md for details.

namespace ArvidsonFoto;