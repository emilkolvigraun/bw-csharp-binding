namespace BWBinding.Control
{
    class Controller
    {
        private static ClientController instance;

        public static ClientController Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ClientController();
                }
                return instance;
            }
        }
    }
}
