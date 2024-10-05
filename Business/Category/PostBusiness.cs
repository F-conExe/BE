using Business.Base;
using Business.DTO.Create;
using Business.DTO.Update;
using Common;
using DATA;
using DATA.Models;
using DATA.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Category
{
    public interface IPostBusiness
    {
        Task<IBusinessResult> GetAll();
        Task<IBusinessResult> GetById(int id);
        Task<IBusinessResult> Save(CreatePostDTO post);
        Task<IBusinessResult> Update(UpdatePostDTO post);
        Task<IBusinessResult> DeleteById(int id);

        Task<IBusinessResult> Search(string skill, decimal? minSalary, decimal? maxSalary, int? postTypeId);

        Task<int> GetPostCount();

    }

    public class PostBusiness : IPostBusiness
    {
        private readonly UnitOfWork _unitOfWork;

        public PostBusiness()
        {
            _unitOfWork ??= new UnitOfWork();
        }

        public async Task<IBusinessResult> DeleteById(int id)
        {
            try
            {
                var post = await _unitOfWork.PostRepo.GetByIdAsync(id);
                if (post != null)
                {
                    bool result = await _unitOfWork.PostRepo.RemoveAsync(post);
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
                var post = await _unitOfWork.PostRepo.GetAllAsync();
                if (post == null || !post.Any())
                {
                    return new BusinessResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA_MSG);
                }
                return new BusinessResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, post);
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
                var currency = await _unitOfWork.PostRepo.GetByIdAsync(id);

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

        public async Task<int> GetPostCount()
        {
            return await _unitOfWork.PostRepo.GetPostCount();
        }

        public async Task<IBusinessResult> Save(CreatePostDTO postdto)
        {
            try
            {
                // Validate if the post type exists
                var existType = await _unitOfWork.PostTypeRepository.GetByIdAsync(postdto.PostTypeId);
                if (existType == null)
                {
                    return new BusinessResult(Const.FAIL_CREATE_CODE, "Invalid Post Type ID");
                }

                var post = new Post
                {
                    UserId = postdto.UserId,
                    PostTypeId = postdto.PostTypeId,
                    Title = postdto.Title,
                    Description = postdto.Description,
                    BudgetOrSalary = postdto.BudgetOrSalary,
                    Skills = postdto.Skills,
                    Status = postdto.Status,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                };

                int result = await _unitOfWork.PostRepo.CreateAsync(post);
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


        public async Task<IBusinessResult> Search(string skill, decimal? minSalary, decimal? maxSalary, int? postTypeId)
        {
            try
            {
                var posts = await _unitOfWork.PostRepo.SearchAsync(skill, minSalary, maxSalary,postTypeId);
                if (posts == null || !posts.Any())
                {
                    return new BusinessResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA_MSG);
                }
                return new BusinessResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, posts);
            }
            catch (Exception ex)
            {
                return new BusinessResult(Const.ERROR_EXCEPTION, ex.ToString());
            }
        }

       

        public async Task<IBusinessResult> Update(UpdatePostDTO post)
        {
            try
            {
                // Validate if the user exists
                var existUser = await _unitOfWork.UserRepository.GetByIdAsync(post.UserId);
                if (existUser == null)
                {
                    return new BusinessResult(Const.FAIL_UPDATE_CODE, "User not found");
                }

                // Validate if the post type exists
                var existType = await _unitOfWork.PostTypeRepository.GetByIdAsync(post.PostTypeId);
                if (existType == null)
                {
                    return new BusinessResult(Const.FAIL_UPDATE_CODE, "Post Type not found");
                }

                // Retrieve the post to be updated
                var existPost = await _unitOfWork.PostRepo.GetByIdAsync(post.PostId);
                if (existPost == null)
                {
                    return new BusinessResult(Const.FAIL_UPDATE_CODE, "Post not found");
                }

                // Update the post properties
                existPost.UserId = post.UserId; // Since UserId is non-nullable
                existPost.PostTypeId = post.PostTypeId; // Since PostTypeId is non-nullable
                existPost.Title = post.Title ?? existPost.Title;
                existPost.Description = post.Description ?? existPost.Description;
                existPost.Skills = post.Skills ?? existPost.Skills;
                existPost.Status = post.Status ?? existPost.Status;
                existPost.BudgetOrSalary = post.BudgetOrSalary ?? existPost.BudgetOrSalary; // Nullable property
                existPost.CreatedAt = post.CreatedAt; // Assuming CreatedAt should be set from DTO
                existPost.UpdatedAt = DateTime.UtcNow; 

                // Save the changes
                int result = await _unitOfWork.PostRepo.UpdateAsync(existPost);
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


        

    }
}
