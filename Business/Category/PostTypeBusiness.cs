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
    public interface IPostTypeBusiness
    {
        Task<IBusinessResult> GetAll();
        Task<IBusinessResult> GetById(int id);
        Task<IBusinessResult> Save(CreatePostTypeDTO postType);
        Task<IBusinessResult> Update(UpdatePostTypeDTO postType);
        Task<IBusinessResult> DeleteById(int id);
    }

    public class PostTypeBusiness : IPostTypeBusiness
    {
        private readonly UnitOfWork _unitOfWork;

        public PostTypeBusiness()
        {
            _unitOfWork ??= new UnitOfWork();
        }

        public async Task<IBusinessResult> DeleteById(int id)
        {
            try
            {
                var post = await _unitOfWork.PostTypeRepository.GetByIdAsync(id);
                if (post != null)
                {
                    bool result = await _unitOfWork.PostTypeRepository.RemoveAsync(post);
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
                var postType = await _unitOfWork.PostTypeRepository.GetAllAsync();

                if (postType == null || !postType.Any())
                {
                    return new BusinessResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA_MSG);
                }

                return new BusinessResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, postType);
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
                var currency = await _unitOfWork.PostTypeRepository.GetByIdAsync(id);

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

        public async Task<IBusinessResult> Save(CreatePostTypeDTO postTypeDTO)
        {
            try
            {
                var postType = new PostType
                {
                    TypeName = postTypeDTO.TypeName,
                    Description = postTypeDTO.Description,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                int result = await _unitOfWork.PostTypeRepository.CreateAsync(postType);
                if(result > 0)
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

        public async Task<IBusinessResult> Update(UpdatePostTypeDTO postTypeDTO)
        {
            try
            {
                PostType existPostType = await _unitOfWork.PostTypeRepository.GetByIdAsync(postTypeDTO.TypeId);
                if (existPostType != null) 
                {
                    existPostType.TypeName = postTypeDTO.TypeName ?? existPostType.TypeName;
                    existPostType.Description = postTypeDTO.Description ?? existPostType.Description;
                    existPostType.CreatedAt = postTypeDTO.CreatedAt;
                    existPostType.UpdatedAt = DateTime.UtcNow;

                    int result = await _unitOfWork.PostTypeRepository.UpdateAsync(existPostType);
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
