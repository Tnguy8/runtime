// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using ILCompiler.DependencyAnalysis.ReadyToRun;
using Internal.Text;

namespace ILCompiler.DependencyAnalysis
{
    class SignatureEmbeddedPointerIndirectionNode : EmbeddedPointerIndirectionNode<Signature>
    {
        private readonly Import _import;
        
        public SignatureEmbeddedPointerIndirectionNode(Import import, Signature signature)
            : base(signature)
        {
            _import = import;
        }

        protected override string GetName(NodeFactory factory) => $"Embedded pointer to {Target.GetMangledName(factory.NameMangler)}";

        public override IEnumerable<DependencyListEntry> GetStaticDependencies(NodeFactory factory)
        {
            return new[]
            {
                new DependencyListEntry(Target, "reloc"),
            };
        }

        public override void EncodeData(ref ObjectDataBuilder dataBuilder, NodeFactory factory, bool relocsOnly)
        {
            dataBuilder.RequireInitialPointerAlignment();
            dataBuilder.EmitReloc(Target, RelocType.IMAGE_REL_BASED_ADDR32NB);
        }
    
        public override void AppendMangledName(NameMangler nameMangler, Utf8StringBuilder sb)
        {
            sb.Append("SignaturePointer_");
            Target.AppendMangledName(nameMangler, sb);
            if (_import.CallSite != null)
            {
                sb.Append(" @ ");
                sb.Append(_import.CallSite);
            }
        }

        public override int ClassCode => -66002498;

        public override int CompareToImpl(ISortableNode other, CompilerComparer comparer)
        {
            return comparer.Compare(_import, ((SignatureEmbeddedPointerIndirectionNode)other)._import);
        }
    }
}
