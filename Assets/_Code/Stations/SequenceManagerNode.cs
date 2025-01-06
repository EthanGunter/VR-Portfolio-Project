using Unity.VisualScripting;
using UnityEngine;

namespace VRPortfolio.VisualScripting
{
    public class SequenceManagerNode : ManualEventUnit<Unit>
    {
        protected override string hookName => nameof(SequenceManagerNode);

        #region Variables

        [DoNotSerialize] ValueInput sequenceManager;
        [DoNotSerialize] ValueOutput index;
        int _index;
        [DoNotSerialize] ValueOutput nauseaRisk;
        bool _nauseaRisk;
        [DoNotSerialize] ValueOutput visited;
        bool _visited;
        [DoNotSerialize] ValueOutput audio;
        AudioClip _audio;

        SequenceManager _seqMan;
        GraphReference _graph;
        Unit _unit;

        #endregion


        protected override void Definition()
        {
            base.Definition();

            sequenceManager = ValueInput<SequenceManager>(nameof(sequenceManager));
            nauseaRisk = ValueOutput<bool>(nameof(nauseaRisk), (flow) => _nauseaRisk);
            visited = ValueOutput<bool>(nameof(visited), (flow) => _visited);
            index = ValueOutput<int>(nameof(index), (flow) => _index);
            audio = ValueOutput<AudioClip>(nameof(audio), (flow) => _audio);
        }

        public override void StartListening(GraphStack stack)
        {
            base.StartListening(stack);
            _graph = stack.AsReference();
            Flow flow = Flow.New(_graph);
            _seqMan = flow.GetValue<SequenceManager>(sequenceManager);
            _seqMan.OnFramePlay += OnFrameChange;
        }
        public override void StopListening(GraphStack stack)
        {
            base.StopListening(stack);
            _seqMan.OnFramePlay -= OnFrameChange;
        }

        private void OnFrameChange(SequenceManager.Frame frame)
        {
            Flow flow = Flow.New(_graph);

            _index = _seqMan.Index;
            _nauseaRisk = frame.nauseaRisk;
            _visited = frame.visited;
            _audio = frame.voiceover;

            Trigger(_graph, _unit);
        }
    }
}