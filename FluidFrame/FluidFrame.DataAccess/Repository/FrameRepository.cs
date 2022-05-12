using FluidFrame.DataAccess.Data;
using FluidFrame.DataAccess.Repository.IRepository;
using FluidFrame.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluidFrame.DataAccess.Repository
{
    public class FrameRepository : Repository<Frame>, IFrameRepository
    {
        private ApplicationDbContext _db;

        public FrameRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Frame obj)
        {
            var frameDb = _db.Frames.FirstOrDefault(u => u.Id == obj.Id);
            if (frameDb != null)
            {
                frameDb.Description = obj.Description;
                frameDb.ModelName = obj.ModelName;
                frameDb.OwnerPhoneNumber = obj.OwnerPhoneNumber;

                frameDb.Price = obj.Price;
                frameDb.Price7Days = obj.Price7Days;

                frameDb.Latitude = obj.Latitude;
                frameDb.Longitude = obj.Longitude;

                frameDb.CategoryId = obj.CategoryId;
                frameDb.FrameTypeId = obj.FrameTypeId;

                if (obj.ImageUrl != null)
                {
                    frameDb.ImageUrl = obj.ImageUrl;
                }
            }
        }
    }
}
