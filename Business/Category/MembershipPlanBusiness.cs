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
    public interface IMembershipPlanBusiness
    {
        Task<IBusinessResult> GetAll();
        Task<IBusinessResult> GetById(int id);
        Task<IBusinessResult> Save(CreateMembershipPlanDTO mbsp);
        Task<IBusinessResult> Update(UpdateMemberhsipPlanDTO mbsp);
        Task<IBusinessResult> DeleteById(int id);
    }

    public class MembershipPlanBusiness : IMembershipPlanBusiness
    {
        private readonly UnitOfWork _unitOfWork;

        public MembershipPlanBusiness()
        {
            _unitOfWork ??= new UnitOfWork();
        }

        public async Task<IBusinessResult> DeleteById(int id)
        {
            try
            {
                var mbsp = await _unitOfWork.PostTypeRepository.GetByIdAsync(id);
                if (mbsp != null)
                {
                    bool result = await _unitOfWork.PostTypeRepository.RemoveAsync(mbsp);
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

        public async Task<IBusinessResult> GetAll()
        {
            try
            {
                var mbsp = await _unitOfWork.MemberShipPlanRepo.GetAllAsync();

                if (mbsp == null || !mbsp.Any())
                {
                    return new BusinessResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA_MSG);
                }

                return new BusinessResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, mbsp);
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
                #region Business rule
                #endregion

                //var currency = await _currencyRepository.GetByIdAsync(code);
                var currency = await _unitOfWork.MemberShipPlanRepo.GetByIdAsync(id);

                if (currency == null)
                {
                    return new BusinessResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA_MSG);
                }
                else
                {
                    return new BusinessResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, currency);
                }
            }
            catch (Exception ex)
            {
                return new BusinessResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<IBusinessResult> Save(CreateMembershipPlanDTO mbspdto)
        {
            try
            {
                var mbsp = new MembershipPlan
                {
                    Name = mbspdto.Name,
                    Description = mbspdto.Description,
                    DurationDays = mbspdto.DurationDays,
                    Features = mbspdto.Features,
                    Price = mbspdto.Price

                };
                int result = await _unitOfWork.MemberShipPlanRepo.CreateAsync(mbsp);
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

        public async Task<IBusinessResult> Update(UpdateMemberhsipPlanDTO mbspdto)
        {

            try
            {
                MembershipPlan existMbsp = await _unitOfWork.MemberShipPlanRepo.GetByIdAsync(mbspdto.PlanId);
                if (existMbsp != null)
                {
                    existMbsp.Name = mbspdto.Name ?? existMbsp.Name;
                    existMbsp.Features = mbspdto.Features ?? existMbsp.Features;
                    existMbsp.Description = mbspdto.Description ?? existMbsp.Description;
                    existMbsp.DurationDays = mbspdto.DurationDays ?? existMbsp.DurationDays ;
                    existMbsp.Price = mbspdto.Price ?? existMbsp.Price;

                    int result = await _unitOfWork.MemberShipPlanRepo.UpdateAsync(existMbsp);
                    if (result > 0)
                    {
                        return new BusinessResult(Const.SUCCESS_UPDATE_CODE, Const.SUCCESS_UPDATE_MSG);
                    }
                    else
                    {
                        return new BusinessResult(Const.FAIL_UPDATE_CODE, Const.FAIL_UPDATE_MSG);
                    }


                }
                else
                {
                    return new BusinessResult(Const.FAIL_UPDATE_CODE, "User not found");
                }

            }
            catch (Exception ex)
            {
                return new BusinessResult(Const.ERROR_EXCEPTION, ex.ToString());
            }
        }
    }
}
