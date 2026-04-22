using Core;
using UniRx;

namespace ZooWorld.UI.DeathCounter
{
    public class DeathCounterPresenter : BaseDisposable
    {
        private readonly DeathCounterModel _model;
        private readonly DeathCounterView _view;

        public DeathCounterPresenter(DeathCounterModel model, DeathCounterView view)
        {
            _model = model;
            _view = view;

            AddDispose(_model);

            if (_view != null)
            {
                AddDispose(_model.PreyDead.Subscribe(v => _view.SetPreyDead(v)));
                AddDispose(_model.PredatorDead.Subscribe(v => _view.SetPredatorDead(v)));
            }
        }
    }
}
