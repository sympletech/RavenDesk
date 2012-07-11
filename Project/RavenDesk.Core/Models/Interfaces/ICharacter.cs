using RavenDesk.Core.Data.Attributes;

namespace RavenDesk.Core.Models.Interfaces
{
    public interface ICharacter
    {
        [RequiredValue(ErrorMessage = "Name Is Required")]
        string Name { get; set; }

        string Description { get; set; }
        string CatchPhrase { get; set; }
        string Sex { get; set; }
        string Id { get; set; }
    }
}