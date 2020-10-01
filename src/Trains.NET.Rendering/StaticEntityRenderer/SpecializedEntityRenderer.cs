using System;

namespace Trains.NET.Rendering
{
    public abstract class SpecializedEntityRenderer<TRenderType, TPublicType> : IStaticEntityRenderer<TPublicType>
        where TRenderType : TPublicType
    {
        bool IRenderer<TPublicType>.ShouldRender(TPublicType entity)
        {
            if (entity is TRenderType itemToRender)
            {
                return ShouldRender(itemToRender);
            }
            return false;
        }

        void IRenderer<TPublicType>.Render(ICanvas canvas, TPublicType entity)
        {
            if (entity is TRenderType itemToRender)
            {
                Render(canvas, itemToRender);
                return;
            }

            throw new InvalidOperationException("Shouldn't be asked to render something we can't render");
        }

        string IStaticEntityRenderer<TPublicType>.GetCacheKey(TPublicType entity)
        {
            if (entity is TRenderType itemToRender)
            {
                return GetCacheKey(itemToRender);
            }

            throw new InvalidOperationException("Shouldn't be asked for a cache key for something we can't render");
        }

        protected abstract string GetCacheKey(TRenderType itemToRender);
        protected abstract void Render(ICanvas canvas, TRenderType item);

        // By default anything will render if its the right type, handled by the explicit method above
        protected virtual bool ShouldRender(TRenderType itemToRender) => true;

    }
}
