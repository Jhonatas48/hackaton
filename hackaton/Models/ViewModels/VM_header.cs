namespace hackaton.Models.ViewModels
{
    public class VM_header
    {
        public enum Variations
        {
            Criar_Perfil = 0,
            Login = 1,
            Logout = 2
        }

        public Variations HeaderVariation { get; set;}
    }
}
