using EShop.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EShop.Application.Service
{
    public interface IMemberService
    {
        Task<Member> AddMemberAsync(Member member);
        Task<Member?> GetMemberAsync(int id);
        Task<List<Member>> GetAllMembersAsync();
        Task<Member> UpdateMemberAsync(Member member);
        Task DeleteMemberAsync(int id);
    }
} 