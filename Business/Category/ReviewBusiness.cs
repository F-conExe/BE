using Business.Base;
using Business.DTO.Create;
using Business.DTO.Update;
using Common;
using DATA.Models;
using DATA;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Business.Category
{
    public interface IReviewBusiness
    {
        Task<IBusinessResult> GetAll();
        Task<IBusinessResult> GetById(int id);
        Task<IBusinessResult> Save(CreateReviewDTO reviewDto,string token);
        Task<IBusinessResult> Update(UpdateReviewDTO reviewDto, string token);
        Task<IBusinessResult> DeleteById(int id);
    }

    public class ReviewBusiness : BaseBusiness, IReviewBusiness
    {
        private readonly UnitOfWork _unitOfWork;

        public ReviewBusiness(IConfiguration configuration) : base(configuration) 
        {
            _unitOfWork ??= new UnitOfWork();
        }

        public async Task<IBusinessResult> GetAll()
        {
            try
            {
                var reviews = await _unitOfWork.ReviewRepo.GetAllAsync();
                if (reviews == null || !reviews.Any())
                {
                    return new BusinessResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA_MSG);
                }

                return new BusinessResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, reviews);
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
                var review = await _unitOfWork.ReviewRepo.GetByIdAsync(id);
                if (review == null)
                {
                    return new BusinessResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA_MSG);
                }

                return new BusinessResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, review);
            }
            catch (Exception ex)
            {
                return new BusinessResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<IBusinessResult> Save(CreateReviewDTO reviewDto, string token)
        {
            try
            {
                // Get the current user from the token
                var currentUserResult = await GetCurrentUser(token);
                if (!(currentUserResult.Data is User currentUser))
                {
                    return new BusinessResult(Const.FAIL_CREATE_CODE, "Failed to authenticate user");
                }

                // Get the post to find the reviewee
                var post = await _unitOfWork.PostRepo.GetByIdAsync(reviewDto.PostId);
                if (post == null)
                {
                    return new BusinessResult(Const.FAIL_CREATE_CODE, "Post not found");
                }

                var review = new Review
                {
                    PostId = reviewDto.PostId,
                    ReviewerId = currentUser.UserId,
                    RevieweeId = post.UserId, // Assuming the post has a UserId property for the post creator
                    Rating = reviewDto.Rating,
                    Comment = reviewDto.Comment,
                    CreatedAt = DateTime.UtcNow // Use current time instead of the one from DTO
                };

                int result = await _unitOfWork.ReviewRepo.CreateAsync(review);
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

        public async Task<IBusinessResult> Update(UpdateReviewDTO reviewDto, string token)
        {
            try
            {
                // Get the current user from the token
                var currentUserResult = await GetCurrentUser(token);
                if (!(currentUserResult.Data is User currentUser))
                {
                    return new BusinessResult(Const.FAIL_UPDATE_CODE, "Failed to authenticate user");
                }

                var existingReview = await _unitOfWork.ReviewRepo.GetByIdAsync(reviewDto.ReviewId);
                if (existingReview == null)
                {
                    return new BusinessResult(Const.WARNING_NO_DATA_CODE, "Review not found");
                }

                // Check if the current user is the owner of the review
                if (existingReview.ReviewerId != currentUser.UserId)
                {
                    return new BusinessResult(Const.FAIL_UPDATE_CODE, "You are not authorized to update this review");
                }

                // Update the review
                existingReview.PostId = reviewDto.PostId;
                existingReview.Rating = reviewDto.Rating;
                existingReview.Comment = reviewDto.Comment;
                // Don't update ReviewerId, RevieweeId, or CreatedAt

                int result = await _unitOfWork.ReviewRepo.UpdateAsync(existingReview);
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
                var review = await _unitOfWork.ReviewRepo.GetByIdAsync(id);
                if (review != null)
                {
                    bool result = await _unitOfWork.ReviewRepo.RemoveAsync(review);
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
