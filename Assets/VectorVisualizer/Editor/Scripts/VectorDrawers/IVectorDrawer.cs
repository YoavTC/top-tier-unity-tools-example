namespace VectorVisualizer
{
    //Use this interface to create new vector drawers
    public interface IVectorDrawer
    {
        //Unique id of the drawer
        public string Id { get; }

        //Name of the drawer that will be displayed in the menu
        public string MenuName { get; }

        //Properties should be displayed in the menu
        public VectorDrawerProperty Properties { get; }

        //Draw function called in OnSceneGUI for drawing the vector
        public void DrawVectorOnSceneView(VectorVisualizeObject visualizeObject, VectorDrawSettings settings);
    }
}