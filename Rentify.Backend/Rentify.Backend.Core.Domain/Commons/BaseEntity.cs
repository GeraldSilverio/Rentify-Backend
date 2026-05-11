namespace Rentify.Backend.Core.Domain.Commons
{
  
    public abstract class BaseEntity
    {
     
        public string? CreatedBy { get; set; }
      
        public string? ModifiedBy { get; set; }
        
        public DateTime CreatedDate { get; set; }
        
        public DateTime ModifiedDate { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive{ get; set; }
    }
}