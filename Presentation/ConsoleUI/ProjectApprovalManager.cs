using Infrastructure.Data;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

using Application.Services;

namespace Presentation.ConsoleUI
{
    public class ProjectApprovalManager
    {
        private readonly ProjectApprovalDbContext _context;
        private User _currentUser;
        private readonly IProjectApprovalService _approvalService;

        public ProjectApprovalManager(ProjectApprovalDbContext context, IProjectApprovalService approvalService)
        {
            _context = context;
            _approvalService = approvalService;
        }


        public void Run()
        {
            SelectUser();

            bool continuar = true;
            while (continuar)
            {
                ShowMainMenu();
                string opcion = Console.ReadLine() ?? "";

                switch (opcion)
                {
                    case "1":
                        CreateNewProject().GetAwaiter().GetResult();
                        break;
                    case "2":
                        ViewMyRequests();
                        break;
                    case "3":
                        ReviewPendingRequests();
                        break;
                    case "4":
                        SelectUser();
                        break;
                    case "5":
                        continuar = false;
                        break;
                    default:
                        Console.WriteLine("Opción no válida. Intente nuevamente.");
                        break;
                }

                if (continuar)
                {
                    Console.WriteLine("\nPresione cualquier tecla para continuar...");
                    Console.ReadKey();
                }
            }
        }

        private void SelectUser()
        {
            Console.Clear();
            Console.WriteLine("=== SELECCIÓN DE USUARIO ===");

            var usuarios = _context.Users.ToList();
            for (int i = 0; i < usuarios.Count; i++)
            {
                var rolName = _context.ApproverRoles.FirstOrDefault(r => r.Id == usuarios[i].Role)?.Name ?? "Sin rol";
                Console.WriteLine($"{i + 1}. {usuarios[i].Name} - {usuarios[i].Email} ({rolName})");
            }

            Console.Write("\nSeleccione un usuario (número): ");
            if (int.TryParse(Console.ReadLine(), out int seleccion) && seleccion >= 1 && seleccion <= usuarios.Count)
            {
                _currentUser = usuarios[seleccion - 1];
                Console.WriteLine($"Usuario seleccionado: {_currentUser.Name}");
            }
            else
            {
                Console.WriteLine("Selección inválida. Utilizando el primer usuario por defecto.");
                _currentUser = usuarios.First();
            }
            ShowMainMenu();
        }

        private void ShowMainMenu()
        {
            var roleName = _currentUser?.Role.ToString() ?? "Sin rol";

            Console.Clear();
            Console.WriteLine($"=== SISTEMA DE APROBACIÓN DE PROYECTOS ===");
            Console.WriteLine($"Usuario actual: {_currentUser?.Name}");
            Console.WriteLine("\nMENU PRINCIPAL:");
            Console.WriteLine("1. Crear un proyecto nuevo");
            Console.WriteLine("2. Ver el estado de mis solicitudes de proyecto");
            Console.WriteLine("3. Revisar solicitudes de proyecto pendientes");
            Console.WriteLine("4. Cambiar de Usuario");
            Console.WriteLine("5. Salir de la aplicación");
            Console.Write("\nSeleccione una opción: ");
        }

        private async Task CreateNewProject()
        {
            Console.Clear();
            Console.WriteLine("=== CREAR NUEVO PROYECTO ===");

            Console.Write("Título del Proyecto: ");
            string title = Console.ReadLine() ?? "";

            Console.Write("Descripción del Proyecto: ");
            string description = Console.ReadLine() ?? "";

            decimal estimatedAmount = 0;
            bool montoValido = false;
            while (!montoValido)
            {
                Console.Write("Monto Estimado del Proyecto (ej. 15000): ");
                montoValido = decimal.TryParse(Console.ReadLine(), out estimatedAmount);
                if (!montoValido)
                    Console.WriteLine("Por favor, ingrese un valor numérico válido.");
            }

            int duration = 0;
            bool duracionValida = false;
            while (!duracionValida)
            {
                Console.Write("Duración en Meses (ej. 6): ");
                duracionValida = int.TryParse(Console.ReadLine(), out duration);
                if (!duracionValida)
                    Console.WriteLine("Por favor, ingrese un valor numérico válido.");
            }

            Console.WriteLine("\nÁreas disponibles:");
            var areas = _context.Areas.ToList();
            foreach (var area in areas)
            {
                Console.WriteLine($"{area.Id}. {area.Name}");
            }

            int areaId = 0;
            bool areaValida = false;
            while (!areaValida)
            {
                Console.Write("Seleccione el ID del Área: ");
                areaValida = int.TryParse(Console.ReadLine(), out areaId);
                areaValida = areaValida && areas.Any(a => a.Id == areaId);
                if (!areaValida)
                    Console.WriteLine("Por favor, seleccione un ID de área válido.");
            }

            Console.WriteLine("\nTipos de Proyecto disponibles:");
            var tipos = _context.ProjectTypes.ToList();
            foreach (var tipo in tipos)
            {
                Console.WriteLine($"{tipo.Id}. {tipo.Name}");
            }

            int projectTypeId = 0;
            bool tipoValido = false;
            while (!tipoValido)
            {
                Console.Write("Seleccione el ID del Tipo de Proyecto: ");
                tipoValido = int.TryParse(Console.ReadLine(), out projectTypeId);
                tipoValido = tipoValido && tipos.Any(t => t.Id == projectTypeId);
                if (!tipoValido)
                    Console.WriteLine("Por favor, seleccione un ID de tipo válido.");
            }

            var projectId = await _approvalService.CreateProjectProposalAsync(title, description, estimatedAmount, duration, areaId, projectTypeId, _currentUser.Id);

            if (projectId != null)
                Console.WriteLine($"Proyecto creado con éxito. ID: {projectId}");
            else
                Console.WriteLine("No se pudo crear el proyecto.");
        }

        private void ViewMyRequests()
        {
            Console.Clear();
            Console.WriteLine("=== MIS SOLICITUDES DE PROYECTO ===");

            var proyectos = _context.ProjectProposals
                .Where(p => p.CreatedBy == _currentUser.Id)
                .Include(p => p.Areas)
                .Include(p => p.ProjectType)
                .OrderBy(p => p.CreatedAt)
                .ToList();

            if (!proyectos.Any())
            {
                Console.WriteLine("No tienes solicitudes de proyecto registradas.");
                return;
            }

            Console.WriteLine("\nListado de Proyectos:");
            Console.WriteLine("-----------------------------------------------------------------");
            Console.WriteLine("| # | Título | Monto | Área | Tipo | Estado |");
            Console.WriteLine("-----------------------------------------------------------------");

            foreach (var item in proyectos.Select((value, index) => new { value, index }))
            {
                var proyecto = item.value;
                int i = item.index;

                var steps = _context.ProjectApprovalSteps
                    .Where(s => s.ProjectProposalId == proyecto.Id)
                    .ToList();

                string estado = "Pending";
                if (steps.Any(s => s.Status == _context.ApprovalStatuses.First(a => a.Name == "Rejected").Id))
                    estado = "Rejected";
                else if (steps.All(s => s.Status == _context.ApprovalStatuses.First(a => a.Name == "Approved").Id))
                    estado = "Approved";

                var areaName = _context.Areas.FirstOrDefault(a => a.Id == proyecto.Area)?.Name ?? "N/A";
                var typeName = _context.ProjectTypes.FirstOrDefault(t => t.Id == proyecto.Type)?.Name ?? "N/A";

                Console.WriteLine($"| {i + 1} | {proyecto.Title.PadRight(15).Substring(0, 15)} | {proyecto.EstimatedAmount.ToString("C")} | {areaName.PadRight(5)} | {typeName.PadRight(5)} | {estado.PadRight(7)} |");
            }


            Console.WriteLine("-----------------------------------------------------------------");

            Console.Write("\nIngrese el número del proyecto para ver los detalles (0 para volver): ");
            string input = Console.ReadLine();

            if (int.TryParse(input, out int index))
            {
                if (index == 0)
                {
                    return;
                }
                else if (index > 0 && index <= proyectos.Count)
                {
                    var proyectoSeleccionado = proyectos[index - 1];
                    ShowProjectDetails(proyectoSeleccionado.Id);
                }
                else
                {
                    Console.WriteLine("Número de proyecto no válido.");
                }
            }
            else
            {
                Console.WriteLine("Entrada no válida.");
            }

        }

        private void ShowProjectDetails(Guid projectId)
        {
            var proyecto = _context.ProjectProposals
                .FirstOrDefault(p => p.Id == projectId);

            if (proyecto == null)
            {
                Console.WriteLine("Proyecto no encontrado.");
                return;
            }

            var areaName = _context.Areas.FirstOrDefault(a => a.Id == proyecto.Area)?.Name ?? "N/A";
            var typeName = _context.ProjectTypes.FirstOrDefault(t => t.Id == proyecto.Type)?.Name ?? "N/A";
            var statusName = _context.ApprovalStatuses.FirstOrDefault(s => s.Id == proyecto.Status)?.Name ?? "N/A";

            Console.Clear();
            Console.WriteLine($"=== DETALLES DEL PROYECTO: {proyecto.Title} ===");
            Console.WriteLine($"ID: {proyecto.Id}");
            Console.WriteLine($"Descripción: {proyecto.Description}");
            Console.WriteLine($"Monto Estimado: {proyecto.EstimatedAmount:C}");
            Console.WriteLine($"Duración Estimada: {proyecto.EstimatedDuration} meses");
            Console.WriteLine($"Área: {areaName}");
            Console.WriteLine($"Tipo: {typeName}");
            Console.WriteLine($"Estado: {statusName}");
            Console.WriteLine($"Fecha de Creación: {proyecto.CreatedAt}");

            Console.WriteLine("\nPasos de Aprobación:");
            var pasos = _context.ProjectApprovalSteps
                .Include(s => s.ApproverRole)
                .Where(s => s.ProjectProposalId == projectId)
                .OrderBy(s => s.StepOrder)
                .ToList();

            if (!pasos.Any())
            {
                Console.WriteLine("No hay pasos de aprobación definidos para este proyecto.");
                return;
            }

            Console.WriteLine("------------------------------------------------------------");
            Console.WriteLine("| Paso | Rol Aprobador | Estado | Fecha Aprobación |");
            Console.WriteLine("------------------------------------------------------------");

            foreach (var paso in pasos)
            {
                string approverRoleName = paso.ApproverRole?.Name ?? "N/A";

                var stepStatusName = _context.ApprovalStatuses
                    .FirstOrDefault(s => s.Id == paso.Status)?.Name ?? "N/A";

                string approvedDate = paso.DecisionDate.HasValue ? paso.DecisionDate.Value.ToString("dd/MM/yyyy") : "Pendiente";

                Console.WriteLine($"| {paso.StepOrder} | {approverRoleName.PadRight(15).Substring(0, 15)} | {stepStatusName.PadRight(10).Substring(0, 10)} | {approvedDate} |");
            }
            Console.WriteLine("------------------------------------------------------------");
        }


        private void ReviewPendingRequests()
        {
            Console.Clear();
            Console.WriteLine("=== REVISAR SOLICITUDES PENDIENTES ===");

            var pendingStatusId = _context.ApprovalStatuses
                .FirstOrDefault(st => st.Name == "Pending")?.Id ?? -1;

            var pendingSteps = _context.ProjectApprovalSteps
                .Include(s => s.ProjectProposal)
                .Include(s => s.ApproverRole)
                .Where(s => s.ApproverRoleId == _currentUser.Role && s.Status == pendingStatusId)
                .OrderBy(s => s.ProjectProposal.CreatedAt)
                .ToList();

            if (!pendingSteps.Any())
            {
                var roleName = _context.ApproverRoles
                    .FirstOrDefault(r => r.Id == _currentUser.Role)?.Name ?? "N/A";
                Console.WriteLine($"No hay solicitudes pendientes para su aprobación como {roleName}.");
                return;
            }

            Console.WriteLine("\nSolicitudes pendientes para su aprobación:");
            Console.WriteLine("------------------------------------------------------------------");
            Console.WriteLine("| # | ID Proyecto | Título          | Monto    | Paso | Rol Aprobador |");
            Console.WriteLine("------------------------------------------------------------------");

            for (int i = 0; i < pendingSteps.Count; i++)
            {
                var step = pendingSteps[i];
                string projectId = step.ProjectProposalId.ToString().Substring(0, 8) + "...";
                string title = (step.ProjectProposal?.Title ?? "N/A").PadRight(15).Substring(0, 15);
                string amount = step.ProjectProposal?.EstimatedAmount.ToString("C") ?? "N/A";
                string approverRole = step.ApproverRole?.Name ?? "N/A";

                Console.WriteLine($"| {i + 1} | {projectId} | {title} | {amount} | {step.StepOrder} | {approverRole} |");
            }
            Console.WriteLine("------------------------------------------------------------------");

            Console.Write("\nSeleccione el número de la solicitud a revisar (0 para salir): ");
            if (int.TryParse(Console.ReadLine(), out int seleccion) && seleccion > 0 && seleccion <= pendingSteps.Count)
            {
                var selectedStep = pendingSteps[seleccion - 1];
                ReviewRequestAsync(selectedStep, _currentUser.Id);
            }
            else if (seleccion != 0)
            {
                Console.WriteLine("Selección no válida.");
            }
        }


        private async Task ReviewRequestAsync(ProjectApprovalStep step, int userId)
        {
            var projectId = step.ProjectProposalId;
            ShowProjectDetails(projectId);

            Console.WriteLine("\n=== REVISAR SOLICITUD ===");
            Console.WriteLine($"Está revisando el paso {step.StepOrder} como {step.ApproverRole?.Name}");
            Console.WriteLine("1. Aprobar");
            Console.WriteLine("2. Rechazar");
            Console.WriteLine("3. Volver");

            Console.Write("\nSeleccione una opción: ");
            var opcion = Console.ReadLine();

            switch (opcion)
            {
                case "1":
                case "2":
                    Console.Write("Ingrese observaciones (opcional): ");
                    string observaciones = Console.ReadLine() ?? "";

                    bool aprobado = opcion == "1";
                    await _approvalService.ProcessApprovalAsync(projectId, aprobado, step.StepOrder, userId, observaciones);

                    Console.WriteLine(aprobado ? "Solicitud aprobada correctamente." : "Solicitud rechazada.");
                    break;
                case "3":
                    break;
                default:
                    Console.WriteLine("Opción no válida.");
                    break;
            }
        }
    }
}