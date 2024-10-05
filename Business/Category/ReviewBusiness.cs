using Business.Base;
using Business.DTO.Create;
using Business.DTO.Update;
using Common;
using DATA.Models;
using DATA;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Business.Category
{
    public interface IReviewBusiness
    {
        Task<IBusinessResult> GetAll();
        Task<IBusinessResult> GetById(int id);
        Task<IBusinessResult> Save(CreateReviewDTO reviewDto, int reviewerId);
        Task<IBusinessResult> Update(UpdateReviewDTO reviewDto);
        Task<IBusinessResult> DeleteById(int id);
    }

    public class ReviewBusiness : IReviewBusiness
    {
        private readonly UnitOfWork _unitOfWork;

        public ReviewBusiness()
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

        public async Task<IBusinessResult> Save(CreateReviewDTO reviewDto, int reviewerId)
        {
            try
            {
                var review = new Review
                {
                    PostId = reviewDto.PostId,
                    ReviewerId = reviewerId, // Use the logged-in user's ID
                    RevieweeId = reviewDto.RevieweeId,
                    Rating = reviewDto.Rating,
                    Comment = reviewDto.Comment,
                    CreatedAt = reviewDto.CreatedAt
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

        public async Task<IBusinessResult> Update(UpdateReviewDTO reviewDto)
        {
            try
            {
                var existingReview = await _unitOfWork.ReviewRepo.GetByIdAsync(reviewDto.ReviewId);
                if (existingReview == null)
                {
                    return new BusinessResult(Const.FAIL_UPDATE_CODE, "Review not found");
                }

                existingReview.PostId = reviewDto.PostId;
                existingReview.ReviewerId = reviewDto.ReviewerId;
                existingReview.RevieweeId = reviewDto.RevieweeId;
                existingReview.Rating = reviewDto.Rating;
                existingReview.Comment = reviewDto.Comment;
                existingReview.CreatedAt = reviewDto.CreatedAt;

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
