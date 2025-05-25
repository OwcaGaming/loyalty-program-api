using EShop.Domain.Models;
using EShop.Domain.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EShop.Application.Service
{
    public class MemberService : IMemberService
    {
        private readonly IMemberRepository _memberRepository;

        public MemberService(IMemberRepository memberRepository)
        {
            _memberRepository = memberRepository;
        }

        public async Task<Member> AddMemberAsync(Member member)
        {
            await _memberRepository.AddAsync(member);
            return member;
        }

        public async Task<Member?> GetMemberAsync(int id)
        {
            return await _memberRepository.GetByIdAsync(id);
        }

        public async Task<List<Member>> GetAllMembersAsync()
        {
            var members = await _memberRepository.GetAllAsync();
            return members.ToList();
        }

        public async Task<Member> UpdateMemberAsync(Member member)
        {
            await _memberRepository.UpdateAsync(member);
            return member;
        }

        public async Task DeleteMemberAsync(int id)
        {
            var member = await _memberRepository.GetByIdAsync(id);
            if (member != null)
            {
                await _memberRepository.DeleteAsync(member);
            }
        }
    }
} 