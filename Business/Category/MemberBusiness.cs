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
using System.Text;
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
    }

    public class MemberBusiness : BaseBusiness, IMemberBusiness
    {
        private readonly UnitOfWork _unitOfWork;

        public MemberBusiness(IConfiguration configuration) : base(configuration)
        {
            _unitOfWork ??= new UnitOfWork();
            configuration = configuration;
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

        public async Task<IBusinessResult> Save(CreateMembershipDTO membershipdto, string token)
        {
            try
            {
                // Get the user ID from the token
                var principal = GetPrincipalFromToken(token);
                if (principal == null)
                {
                    return new BusinessResult(Const.FAIL_VALIDATION_CODE, "Invalid token");
                }

                var username = principal.Identity.Name;
                var user = await _unitOfWork.UserRepository.FindByUsernameAsync(username);
                if (user == null)
                {
                    return new BusinessResult(Const.WARNING_NO_DATA_CODE, "User not found");
                }

                // Validate if the membership plan exists
                var membershipPlan = await _unitOfWork.MemberShipPlanRepo.GetByIdAsync(membershipdto.Planid);
                if (membershipPlan == null)
                {
                    return new BusinessResult(Const.FAIL_CREATE_CODE, "Membership plan not found");
                }

                // Create the membership entity
                var membership = new Membership
                {
                    UserId = user.UserId,
                    Status = membershipdto.Status
                    
                };

                // Create the membership plan assignment entity
                var membershipPlanAssignment = new MembershipPlanAssignment
                {
                    Membership = membership,
                    PlanId = membershipdto.Planid,
                    StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
                    Status = "Active"
                };

                // Set the end date based on the plan type
                if (membershipPlan.Name == "Premium")
                {
                    membershipPlanAssignment.EndDate = membershipPlanAssignment.StartDate.AddMonths(6);
                }
                else if (membershipPlan.Name == "Platinum")
                {
                    membershipPlanAssignment.EndDate = membershipPlanAssignment.StartDate.AddYears(1);
                }

                // Add the plan assignment to the membership
                membership.MembershipPlanAssignments.Add(membershipPlanAssignment);

                // Save the membership and assignment
                int result = await _unitOfWork.MembershipRepo.CreateAsync(membership);
                if (result > 0)
                {
                    return new BusinessResult(Const.SUCCESS_CREATE_CODE, Const.SUCCESS_CREATE_MSG);
                }
                else
                {
                    return new BusinessResult(Const.FAIL_CREATE_CODE, Const.FAIL_CREATE_MSG);
                }
            }
            catch (Exception ex)
            {
                return new BusinessResult(Const.ERROR_EXCEPTION, ex.ToString());
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
                if (existMembership == null)
                {
                    return new BusinessResult(Const.FAIL_UPDATE_CODE, "User not found");
                }

                var existPlan = await _unitOfWork.MemberShipPlanRepo.GetByIdAsync(membership.PlanId);
                if(existPlan == null)
                {
                    return new BusinessResult(Const.FAIL_UPDATE_CODE, "Plan not found");
                }

                existMembership.UserId = membership.UserId;
               
                existMembership.Status = membership.Status;
                

                int result = await _unitOfWork.MembershipRepo.UpdateAsync(existMembership);
                if (result > 0)
                {
                    return new BusinessResult(Const.SUCCESS_UPDATE_CODE, Const.SUCCESS_UPDATE_MSG);
                }
                else
                {
                    return new BusinessResult(Const.FAIL_UPDATE_CODE, Const.FAIL_UPDATE_MSG);
                }
            }
            catch (Exception ex)
            {
                return new BusinessResult(Const.ERROR_EXCEPTION, ex.ToString());
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
                    if (result)
                    {
                        return new BusinessResult(Const.SUCCESS_DELETE_CODE, Const.SUCCESS_DELETE_MSG);
                    }

                    return new BusinessResult(Const.FAIL_DELETE_CODE, Const.FAIL_DELETE_MSG);
                }

                return new BusinessResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA_MSG);
            }
            catch (Exception ex)
            {
                return new BusinessResult(Const.ERROR_EXCEPTION, ex.ToString());
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

                var username = principal.Identity.Name;
                var user = await _unitOfWork.UserRepository.FindByUsernameAsync(username);
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
                    // Update the status if the plan's end date is past or null
                    foreach (var mpa in membership.MembershipPlanAssignments)
                    {
                        if (mpa.EndDate == null || mpa.EndDate < DateOnly.FromDateTime(DateTime.UtcNow))
                        {
                            mpa.Status = "Not Active";
                        }
                    }

                    // Map the updated membership to DTO
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
                return new BusinessResult(Const.ERROR_EXCEPTION, ex.ToString());
            }
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
