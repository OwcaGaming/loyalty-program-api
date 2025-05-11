using EShopDomain.Models;
using EShop.Domain.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EShop.Application.Service
{
    public class MemberService : IMemberService
    {
        private readonly IRepository _repository;
        public MemberService(IRepository repository)
        {
            _repository = repository;
        }
        public Task<Member> AddMemberAsync(Member member) => _repository.AddMemberAsync(member);
        public Task<Member?> GetMemberAsync(int id) => _repository.GetMemberAsync(id);
        public Task<List<Member>> GetAllMembersAsync() => _repository.GetAllMembersAsync();
        public Task<Member> UpdateMemberAsync(Member member) => _repository.UpdateMemberAsync(member);
        public Task DeleteMemberAsync(int id) => _repository.DeleteMemberAsync(id);
    }
} 