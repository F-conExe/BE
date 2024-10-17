using Business.Base;
using Business.DTO.Create;
using Business.DTO.Get;
using Business.DTO.Update;
using Common;
using DATA;
using DATA.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Business.Category
{
    public interface IMemberBusiness
    {
        Task<IBusinessResult> GetAll();
        Task<IBusinessResult> GetById(int id);
        Task<IBusinessResult> Save(CreateMembershipDTO membership, string token);
        Task<IBusinessResult> Update(UpdateMembershipDTO membership);
        Task<IBusinessResult> DeleteById(int id);
        Task<IBusinessResult> GetMembershipPlanByUser(string token);
        Task<IBusinessResult> InitiateMembershipPayment(CreateMembershipDTO membershipDto, string token);
        Task<IBusinessResult> HandleSuccessfulPayment(string userId, int planId);
    }

    public class MemberBusiness : BaseBusiness, IMemberBusiness
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IPayOSService _payOSService;

        public MemberBusiness(IConfiguration configuration, IPayOSService payOSService) : base(configuration)
        {
            _unitOfWork = new UnitOfWork();
            _payOSService = payOSService;
        }

        public async Task<IBusinessResult> GetAll()
        {
            try
            {
                var memberships = await _unitOfWork.MembershipRepo.GetAllAsync();
                if (memberships == null || !memberships.Any())
                {
                    return new BusinessResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA_MSG);
                }

                return new BusinessResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, memberships);
            }
            catch (Exception ex)
            {
                return new BusinessResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<IBusinessResult> GetById(int id)
        {
            try
            {
                var membership = await _unitOfWork.MembershipRepo.GetByIdAsync(id);
                if (membership == null)
                {
                    return new BusinessResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA_MSG);
                }

                return new BusinessResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, membership);
            }
            catch (Exception ex)
            {
                return new BusinessResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<IBusinessResult> Save(CreateMembershipDTO membershipDto, string token)
        {
            try
            {
                var principal = GetPrincipalFromToken(token);
                if (principal == null)
                {
                    return new BusinessResult(Const.FAIL_VALIDATION_CODE, "Invalid token");
                }

                var user = await _unitOfWork.UserRepository.FindByUsernameAsync(principal.Identity.Name);
                if (user == null)
                {
                    return new BusinessResult(Const.WARNING_NO_DATA_CODE, "User not found");
                }

                var membershipPlan = await _unitOfWork.MemberShipPlanRepo.GetByIdAsync(membershipDto.Planid);
                if (membershipPlan == null)
                {
                    return new BusinessResult(Const.FAIL_CREATE_CODE, "Membership plan not found");
                }

                var membership = CreateMembership(user.UserId, membershipDto.Status);
                var membershipPlanAssignment = CreateMembershipPlanAssignment(membership, membershipDto.Planid, membershipPlan.Name);
                membership.MembershipPlanAssignments.Add(membershipPlanAssignment);

                int result = await _unitOfWork.MembershipRepo.CreateAsync(membership);
                return result > 0
                    ? new BusinessResult(Const.SUCCESS_CREATE_CODE, Const.SUCCESS_CREATE_MSG)
                    : new BusinessResult(Const.FAIL_CREATE_CODE, Const.FAIL_CREATE_MSG);
            }
            catch (Exception ex)
            {
                return new BusinessResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<IBusinessResult> Update(UpdateMembershipDTO membership)
        {
            try
            {
                var existMembership = await _unitOfWork.MembershipRepo.GetByIdAsync(membership.MembershipId);
                if (existMembership == null)
                {
                    return new BusinessResult(Const.FAIL_UPDATE_CODE, "Membership not found");
                }

                var existUser = await _unitOfWork.UserRepository.GetByIdAsync(membership.UserId);
                if (existUser == null)
                {
                    return new BusinessResult(Const.FAIL_UPDATE_CODE, "User not found");
                }

                var existPlan = await _unitOfWork.MemberShipPlanRepo.GetByIdAsync(membership.PlanId);
                if (existPlan == null)
                {
                    return new BusinessResult(Const.FAIL_UPDATE_CODE, "Plan not found");
                }

                existMembership.UserId = membership.UserId;
                existMembership.Status = membership.Status;

                int result = await _unitOfWork.MembershipRepo.UpdateAsync(existMembership);
                return result > 0
                    ? new BusinessResult(Const.SUCCESS_UPDATE_CODE, Const.SUCCESS_UPDATE_MSG)
                    : new BusinessResult(Const.FAIL_UPDATE_CODE, Const.FAIL_UPDATE_MSG);
            }
            catch (Exception ex)
            {
                return new BusinessResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<IBusinessResult> DeleteById(int id)
        {
            try
            {
                var membership = await _unitOfWork.MembershipRepo.GetByIdAsync(id);
                if (membership != null)
                {
                    bool result = await _unitOfWork.MembershipRepo.RemoveAsync(membership);
                    return result
                        ? new BusinessResult(Const.SUCCESS_DELETE_CODE, Const.SUCCESS_DELETE_MSG)
                        : new BusinessResult(Const.FAIL_DELETE_CODE, Const.FAIL_DELETE_MSG);
                }

                return new BusinessResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA_MSG);
            }
            catch (Exception ex)
            {
                return new BusinessResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<IBusinessResult> GetMembershipPlanByUser(string token)
        {
            try
            {
                var principal = GetPrincipalFromToken(token);
                if (principal == null)
                {
                    return new BusinessResult(Const.FAIL_VALIDATION_CODE, "Invalid token or user not found.");
                }

                var user = await _unitOfWork.UserRepository.FindByUsernameAsync(principal.Identity.Name);
                if (user == null)
                {
                    return new BusinessResult(Const.FAIL_VALIDATION_CODE, "User not found.");
                }

                var memberships = await _unitOfWork.MembershipRepo.GetAllByUserIdAsync(user.UserId);
                if (memberships == null || !memberships.Any())
                {
                    return new BusinessResult(Const.WARNING_NO_DATA_CODE, "User has no membership plans");
                }

                var membershipDTOs = memberships.Select(membership =>
                {
                    foreach (var mpa in membership.MembershipPlanAssignments)
                    {
                        if (mpa.EndDate == null || mpa.EndDate < DateOnly.FromDateTime(DateTime.UtcNow))
                        {
                            mpa.Status = "Not Active";
                        }
                    }
                    return MapToDTO(membership);
                }).ToList();

                var userMembershipsDTO = new UserMembershipsDTO
                {
                    UserId = user.UserId,
                    Memberships = membershipDTOs
                };

                return new BusinessResult(Const.SUCCESS_READ_CODE, "Membership plans retrieved successfully", userMembershipsDTO);
            }
            catch (Exception ex)
            {
                return new BusinessResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<IBusinessResult> InitiateMembershipPayment(CreateMembershipDTO membershipDto, string token)
        {
            try
            {
                var principal = GetPrincipalFromToken(token);
                if (principal == null) return new BusinessResult(Const.FAIL_VALIDATION_CODE, "Invalid token");

                var user = await _unitOfWork.UserRepository.FindByUsernameAsync(principal.Identity.Name);
                if (user == null) return new BusinessResult(Const.WARNING_NO_DATA_CODE, "User not found");

                var membershipPlan = await _unitOfWork.MemberShipPlanRepo.GetByIdAsync(membershipDto.Planid);
                if (membershipPlan == null) return new BusinessResult(Const.FAIL_CREATE_CODE, "Membership plan not found");

                var paymentResult = await _payOSService.RequestWithPayOsAsync(user.UserId.ToString(), membershipPlan.Price);
                if (paymentResult.Status != 201) return new BusinessResult(Const.FAIL_CREATE_CODE, "Failed to create payment URL");

                // Store the pending membership information in a temporary storage or cache (implementation needed)

                return paymentResult;
            }
            catch (Exception ex)
            {
                return new BusinessResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<IBusinessResult> HandleSuccessfulPayment(string userId, int planId)
        {
            try
            {
                var user = await _unitOfWork.UserRepository.GetByIdAsync(int.Parse(userId));
                if (user == null) return new BusinessResult(Const.WARNING_NO_DATA_CODE, "User not found");

                var membershipPlan = await _unitOfWork.MemberShipPlanRepo.GetByIdAsync(planId);
                var membership = CreateMembership(user.UserId, "Active");
                var membershipPlanAssignment = CreateMembershipPlanAssignment(membership, planId, membershipPlan.Name);
                membership.MembershipPlanAssignments.Add(membershipPlanAssignment);

                int result = await _unitOfWork.MembershipRepo.CreateAsync(membership);
                return result > 0
                    ? new BusinessResult(Const.SUCCESS_CREATE_CODE, Const.SUCCESS_CREATE_MSG)
                    : new BusinessResult(Const.FAIL_CREATE_CODE, Const.FAIL_CREATE_MSG);
            }
            catch (Exception ex)
            {
                return new BusinessResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        private Membership CreateMembership(int userId, string status) =>
            new Membership { UserId = userId, Status = status };

        private MembershipPlanAssignment CreateMembershipPlanAssignment(Membership membership, int planId, string planName)
        {
            var assignment = new MembershipPlanAssignment
            {
                Membership = membership,
                PlanId = planId,
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
                Status = "Active"
            };

            assignment.EndDate = planName switch
            {
                "Premium" => assignment.StartDate.AddMonths(6),
                "Platinum" => assignment.StartDate.AddYears(1),
                _ => assignment.EndDate
            };

            return assignment;
        }

        private MembershipDTO MapToDTO(Membership membership)
        {
            return new MembershipDTO
            {
                MembershipId = membership.MembershipId,
                UserId = membership.UserId,
                Status = membership.Status,
                MembershipPlanAssignments = membership.MembershipPlanAssignments?
                    .Select(mpa => new MembershipPlanAssignmentDTO
                    {
                        PlanAssignmentId = mpa.AssignmentId,
                        PlanId = (int)mpa.PlanId,
                        PlanName = mpa.Plan?.Name,
                        StartDate = mpa.StartDate,
                        EndDate = mpa.EndDate,
                        Status = mpa.Status
                    })
                    .ToList() ?? new List<MembershipPlanAssignmentDTO>()
            };
        }
    }
}
