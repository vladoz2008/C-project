using System.IO;

namespace LibraryInformationSystem
{
    
    public interface IEntity
    {
        void WriteToBinary(BinaryWriter writer);
    }
}
