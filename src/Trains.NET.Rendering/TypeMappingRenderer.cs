namespace Trains.NET.Rendering
{
    public abstract class TypeMappingRenderer<TRenderType, TPublicType> : IRenderer<TPublicType>
        where TRenderType : TPublicType
    {
        void IRenderer<TPublicType>.Render(ICanvas canvas, TPublicType entity)
        {
            if (entity is TRenderType itemToRender)
            {
                Render(canvas, itemToRender);
            }
        }

        bool IRenderer<TPublicType>.ShouldRender(TPublicType entity)
        {
            if (entity is TRenderType itemToRender)
            {
                return ShouldRender(itemToRender);
            }
            return false;
        }

        protected abstract bool ShouldRender(TRenderType itemToRender);
        protected abstract void Render(ICanvas canvas, TRenderType item);
    }
}
