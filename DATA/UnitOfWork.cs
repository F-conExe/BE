using DATA.Models;
using DATA.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DATA
{
    public class UnitOfWork : IDisposable
    {
        private UserRepo _userRepository;
        private PostTypeRepo _postTypeRepository;
        private MemberShipPlanRepo _memberShipPlanRepository;
        private MembershipPlanAsmRepo _membershipPlanAsmRepo;
        private PostRepo _postRepo;
        private MembershipRepo _membershipRepo;
        private ReviewRepo _reviewRepo;
        private exe201cContext _context;
        private bool _disposed = false;

        public UnitOfWork()
        {
            _context ??= new exe201cContext();
        }

        public UserRepo UserRepository
        {
            get
            {
                return _userRepository ??= new UserRepo(_context);
            }
        }

        public PostTypeRepo PostTypeRepository
        {
            get
            {
                return _postTypeRepository ??= new PostTypeRepo(_context);
            }
        }

        public MemberShipPlanRepo MemberShipPlanRepo
         {
            get
            {
                return _memberShipPlanRepository ??= new MemberShipPlanRepo(_context);
            }
         }

        public PostRepo PostRepo
        {
            get 
            {
                return _postRepo ??= new PostRepo(_context);     
            }
        }

        public MembershipRepo MembershipRepo
        {
            get
            {
                return _membershipRepo ??= new MembershipRepo(_context);
            }
        }

        public MembershipPlanAsmRepo MembershipPlanAsmRepo
        {
            get
            {
                return _membershipPlanAsmRepo ??= new MembershipPlanAsmRepo(_context);
            }
        }

        public ReviewRepo ReviewRepo
        {
            get
            {
                return _reviewRepo ??= new ReviewRepo(_context);
            }
        }

      

        public void Save()
        {
            _context.SaveChanges();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
