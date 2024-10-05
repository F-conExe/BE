using Business.Base;
using Business.DTO.Create;
using Business.DTO.Update;
using Common;
using DATA;
using DATA.Models;
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
        Task<IBusinessResult> Save(CreateMembershipDTO membership);
        Task<IBusinessResult> Update(UpdateMembershipDTO membership);
        Task<IBusinessResult> DeleteById(int id);
    }

    public class MemberBusiness : IMemberBusiness
    {
        private readonly UnitOfWork _unitOfWork;

        public MemberBusiness()
        {
            _unitOfWork ??= new UnitOfWork();
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

        public async Task<IBusinessResult> Save(CreateMembershipDTO membershipdto)
        {
            try
            {
                var membership = new Membership
                {
                    UserId = membershipdto.UserId,
                    Status = membershipdto.Status,
                };

                var membershipPlan = await _unitOfWork.MemberShipPlanRepo.GetByIdAsync(membershipdto.Planid);
                if (membershipPlan == null)
                {
                    return new BusinessResult(Const.FAIL_CREATE_CODE, "Membership plan not found");
                }

                var membershipPlanAssignment = new MembershipPlanAssignment
                {
                    Membership = membership,
                    PlanId = membershipdto.Planid,
                    StartDate = DateOnly.FromDateTime(DateTime.Now),
                    Status = "Active"
                };

                if (membershipPlan.Name == "Premium")
                {
                    membershipPlanAssignment.EndDate = membershipPlanAssignment.StartDate.AddMonths(6);
                }
                else if (membershipPlan.Name == "Platinum")
                {
                    membershipPlanAssignment.EndDate = membershipPlanAssignment.StartDate.AddYears(1);
                }

                membership.MembershipPlanAssignments.Add(membershipPlanAssignment);

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
    }
}
