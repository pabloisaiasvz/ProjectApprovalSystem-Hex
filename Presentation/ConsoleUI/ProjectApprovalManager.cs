
using Application.DTOs;
using Application.Services;
using Domain.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Presentation.ConsoleUI
{
    public class ProjectApprovalManager
    {
        private readonly IUserQueries _userQueries;
        private readonly IProjectProposalQueries _projectQueries;
        private readonly IPendingApprovalQueries _pendingApprovalQueries;
        private readonly ICatalogQueries _catalogQueries;
        private readonly IProjectCreateService _createService;
        private readonly IProjectApprovalProcessorService _approvalService;

        private UserDto _currentUser;

        public ProjectApprovalManager(
            IUserQueries userQueries,
            IProjectProposalQueries projectQueries,
            IPendingApprovalQueries pendingApprovalQueries,
            ICatalogQueries catalogQueries,
            IProjectCreateService createService,
            IProjectApprovalProcessorService approvalService)
        {
            _userQueries = userQueries;
            _projectQueries = projectQueries;
            _pendingApprovalQueries = pendingApprovalQueries;
            _catalogQueries = catalogQueries;
            _createService = createService;
            _approvalService = approvalService;
        }

        public async Task RunAsync()
        {
            await SelectUserAsync();

            bool continuar = true;
            while (continuar)
            {
                ShowMainMenu();
                string opcion = Console.ReadLine() ?? "";

                try
                {
                    switch (opcion)
                    {
                        case "1":
                            await CreateNewProjectAsync();
                            break;
                        case "2":
                            await ViewMyRequestsAsync();
                            break;
                        case "3":
                            await ReviewPendingRequestsAsync();
                            break;
                        case "4":
                            await ReviewRejectedProjectsAsync();
                            break;
                        case "5":
                            await SelectUserAsync();
                            break;
                        case "6":
                            continuar = false;
                            break;
                        default:
                            Console.WriteLine("Opción no válida. Intente nuevamente.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }

                if (continuar)
                {
                    Console.ReadKey();
                }
            }
        }

        private async Task SelectUserAsync()
        {
            Console.Clear();
            Console.WriteLine("=== SELECCIÓN DE USUARIO ===");

            var usuarios = await _userQueries.GetAllUsersAsync();

            for (int i = 0; i < usuarios.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {usuarios[i].Name} - {usuarios[i].Email} ({usuarios[i].RoleName})");
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
        }

        private void ShowMainMenu()
        {
            Console.Clear();
            Console.WriteLine($"=== SISTEMA DE APROBACIÓN DE PROYECTOS ===");
            Console.WriteLine($"Usuario actual: {_currentUser?.Name}");
            Console.WriteLine("\nMENU PRINCIPAL:");
            Console.WriteLine("1. Crear un proyecto nuevo");
            Console.WriteLine("2. Ver el estado de mis solicitudes de proyecto");
            Console.WriteLine("3. Revisar solicitudes de proyecto pendientes");
            Console.WriteLine("4. Revisar proyectos rechazados");
            Console.WriteLine("5. Cambiar de Usuario");
            Console.WriteLine("6. Salir de la aplicación");

            Console.Write("\nSeleccione una opción: ");
        }

        private async Task CreateNewProjectAsync()
        {
            Console.Clear();
            Console.WriteLine("=== CREAR NUEVO PROYECTO ===");

            Console.Write("Título del Proyecto: ");
            string title = Console.ReadLine() ?? "";

            Console.Write("Descripción del Proyecto: ");
            string description = Console.ReadLine() ?? "";

            decimal estimatedAmount = await GetValidDecimalInputAsync("Monto Estimado del Proyecto (ej. 15000): ");
            int duration = await    GetValidIntInputAsync("Duración en Meses (ej. 6): ");

            var areas = await _catalogQueries.GetAllAreasAsync();
            int areaId = await SelectFromCatalogAsync("Áreas disponibles:",
            areas.Select(a => (a.Id, a.Name)).ToList());

            var projectTypes = await _catalogQueries.GetAllProjectTypesAsync();
            int projectTypeId = await SelectFromCatalogAsync("Tipos de Proyecto disponibles:",
                 projectTypes.Select(pt => (pt.Id, pt.Name)).ToList());

            var projectId = await _createService.CreateProjectProposalAsync(title, description, estimatedAmount, duration, areaId, projectTypeId, _currentUser.Id);

            if (projectId != null)
                Console.WriteLine($"Proyecto creado con éxito. ID: {projectId}");
            else
                Console.WriteLine("No se pudo crear el proyecto.");
        }

        private async Task<int> GetValidIntInputAsync(string prompt)
        {
            int result;
            Console.Write(prompt);
            while (!int.TryParse(Console.ReadLine(), out result))
            {
                Console.WriteLine("Por favor, ingrese un número válido.");
                Console.Write(prompt);
            }
            return result;
        }

        private async Task<int> SelectFromCatalogAsync(string prompt, List<(int Id, string Name)> items)
        {
            Console.WriteLine(prompt);
            for (int i = 0; i < items.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {items[i].Name}");
            }
            Console.Write("\nSeleccione una opción (número): ");
            while (true)
            {
                if (int.TryParse(Console.ReadLine(), out int selection) && selection >= 1 && selection <= items.Count)
                {
                    return items[selection - 1].Id;
                }
                Console.WriteLine("Selección inválida. Intente nuevamente.");
            }
        }


        private async Task ViewMyRequestsAsync()
        {
            Console.Clear();
            Console.WriteLine("=== MIS SOLICITUDES DE PROYECTO ===");

            var proyectos = await _projectQueries.GetProjectsByUserIdAsync(_currentUser.Id);

            if (!proyectos.Any())
            {
                Console.WriteLine("No tienes solicitudes de proyecto registradas.");
                return;
            }

            Console.WriteLine("\nListado de Proyectos:");
            Console.WriteLine("-----------------------------------------------------------------");
            Console.WriteLine("| # | Título | Monto | Área | Tipo | Estado |");
            Console.WriteLine("-----------------------------------------------------------------");

            for (int i = 0; i < proyectos.Count; i++)
            {
                var proyecto = proyectos[i];
                Console.WriteLine($"| {i + 1} | {proyecto.Title.PadRight(15).Substring(0, Math.Min(15, proyecto.Title.Length))} | {proyecto.EstimatedAmount:C} | {proyecto.AreaName.PadRight(5)} | {proyecto.TypeName.PadRight(5)} | {proyecto.Status.PadRight(7)} |");
            }

            Console.WriteLine("-----------------------------------------------------------------");

            Console.Write("\nIngrese el número del proyecto para ver los detalles (0 para volver): ");
            if (int.TryParse(Console.ReadLine(), out int index))
            {
                if (index == 0)
                    return;
                else if (index > 0 && index <= proyectos.Count)
                {
                    var proyectoSeleccionado = proyectos[index - 1];
                    await ShowProjectDetailsAsync(proyectoSeleccionado.Id);
                }
                else
                {
                    Console.WriteLine("Número de proyecto no válido.");
                }
            }
            else
            {
                Console.WriteLine("Opción no válida.");
            }
        }

        private async Task ShowProjectDetailsAsync(Guid projectId)
        {
            var proyecto = await _projectQueries.GetProjectDetailsByIdAsync(projectId);

            if (proyecto == null)
            {
                Console.WriteLine("Proyecto no encontrado.");
                return;
            }

            Console.Clear();
            Console.WriteLine($"=== DETALLES DEL PROYECTO: {proyecto.Title} ===");
            Console.WriteLine($"ID: {proyecto.Id}");
            Console.WriteLine($"Descripción: {proyecto.Description}");
            Console.WriteLine($"Monto Estimado: {proyecto.EstimatedAmount:C}");
            Console.WriteLine($"Duración Estimada: {proyecto.EstimatedDuration} meses");
            Console.WriteLine($"Área: {proyecto.AreaName}");
            Console.WriteLine($"Tipo: {proyecto.TypeName}");
            Console.WriteLine($"Estado: {proyecto.StatusName}");
            Console.WriteLine($"Fecha de Creación: {proyecto.CreatedAt}");

            var pasos = await _projectQueries.GetProjectApprovalStepsAsync(projectId);

            Console.WriteLine("\nPasos de Aprobación:");
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
                string approvedDate = paso.DecisionDate.HasValue ? paso.DecisionDate.Value.ToString("dd/MM/yyyy") : "Pendiente";
                Console.WriteLine($"| {paso.StepOrder} | {paso.ApproverRoleName.PadRight(15).Substring(0, Math.Min(15, paso.ApproverRoleName.Length))} | {paso.StatusName.PadRight(10).Substring(0, Math.Min(10, paso.StatusName.Length))} | {approvedDate} |");
            }
            Console.WriteLine("------------------------------------------------------------");
        }

        private async Task ReviewPendingRequestsAsync()
        {
            Console.Clear();
            Console.WriteLine("=== REVISAR SOLICITUDES PENDIENTES ===");

            var pendingApprovals = await _pendingApprovalQueries.GetPendingApprovalsByRoleAsync(_currentUser.Role);

            if (!pendingApprovals.Any())
            {
                Console.WriteLine($"No hay solicitudes pendientes para su aprobación como {_currentUser.RoleName}.");
                return;
            }

            Console.WriteLine("\nSolicitudes pendientes para su aprobación:");
            Console.WriteLine("------------------------------------------------------------------");
            Console.WriteLine("| # | ID Proyecto | Título          | Monto    | Paso | Rol Aprobador |");
            Console.WriteLine("------------------------------------------------------------------");

            for (int i = 0; i < pendingApprovals.Count; i++)
            {
                var approval = pendingApprovals[i];
                string projectId = approval.ProjectProposalId.ToString().Substring(0, 8) + "...";
                string title = approval.ProjectTitle.PadRight(15).Substring(0, Math.Min(15, approval.ProjectTitle.Length));

                Console.WriteLine($"| {i + 1} | {projectId} | {title} | {approval.EstimatedAmount:C} | {approval.StepOrder} | {approval.ApproverRoleName} |");
            }
            Console.WriteLine("------------------------------------------------------------------");

            Console.Write("\nSeleccione el número de la solicitud a revisar (0 para salir): ");
            if (int.TryParse(Console.ReadLine(), out int seleccion) && seleccion > 0 && seleccion <= pendingApprovals.Count)
            {
                var selectedApproval = pendingApprovals[seleccion - 1];
                await ReviewRequestAsync(selectedApproval);
            }
            else if (seleccion != 0)
            {
                Console.WriteLine("Selección no válida.");
            }
        }

        private async Task ReviewRequestAsync(PendingApprovalDto pendingApproval)
        {
            await ShowProjectDetailsAsync(pendingApproval.ProjectProposalId);

            Console.WriteLine("\n=== REVISAR SOLICITUD ===");
            Console.WriteLine($"Está revisando el paso {pendingApproval.StepOrder} como {pendingApproval.ApproverRoleName}");
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
                    await _approvalService.ProcessApprovalAsync(pendingApproval.ProjectProposalId, aprobado, pendingApproval.StepOrder, _currentUser.Id, observaciones);

                    Console.WriteLine(aprobado ? "Solicitud aprobada correctamente." : "Solicitud rechazada.");
                    break;
                case "3":
                    break;
                default:
                    Console.WriteLine("Opción no válida.");
                    break;
            }
        }

        private async Task<decimal> GetValidDecimalInputAsync(string prompt)
        {
            decimal result = 0;
            bool isValid = false;
            while (!isValid)
            {
                Console.Write(prompt);
                isValid = decimal.TryParse(Console.ReadLine(), out result);
                if (!isValid)
                    Console.WriteLine("Por favor, ingrese un valor numérico válido.");
            }
            return result;
        }

        private async Task ReviewRejectedProjectsAsync()
        {
            var rechazados = await _approvalService.GetProposalsByStatusNameAsync("Rejected");

            if (!rechazados.Any())
            {
                Console.WriteLine("No hay proyectos rechazados.");
            }
            else
            {
                Console.WriteLine("Proyectos rechazados:");
                for (int i = 0; i < rechazados.Count; i++)
                    Console.WriteLine($"{i + 1}. {rechazados[i].Title}");

                Console.Write("Seleccione un proyecto para marcar como 'En revisión' (0 para cancelar): ");
                if (int.TryParse(Console.ReadLine(), out int opcion) && opcion > 0 && opcion <= rechazados.Count)
                {
                    var proyecto = rechazados[opcion - 1];
                    await _approvalService.MarkAsObservedAsync(proyecto.Id);
                    Console.WriteLine("El proyecto fue marcado como 'En revisión' y ya puede ser evaluado nuevamente desde las solicitudes pendientes.");
                }
            }

        }


    }
}