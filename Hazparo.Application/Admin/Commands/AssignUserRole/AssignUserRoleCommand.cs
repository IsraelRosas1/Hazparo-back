using MediatR;

namespace Hazparo.Application.Admin.Commands.AssignUserRole;

public class AssignUserRoleCommand: IRequest
{
    public string UserEmail {get;set;} = default!;
    public string RoleName {get;set;} =default!;
}