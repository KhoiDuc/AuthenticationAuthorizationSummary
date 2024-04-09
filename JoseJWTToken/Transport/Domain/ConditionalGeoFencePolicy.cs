using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;

namespace JoseJWTToken.Transport.Domain
{
    public class ConditionalGeoFencePolicy : IPolicy
    {
        public string Type { get; set; }

        [DefaultValue(false)]
        [JsonProperty("deny_rooted_jailbroken")]
        public bool? DenyRootedJailbroken { get; set; }

        [DefaultValue(false)]
        [JsonProperty("deny_emulator_simulator")]
        public bool? DenyEmulatorSimulator { get; set; }

        [JsonProperty("fences")]
        public List<IFence> Fences { get; set; }

        [JsonProperty("inside")]
        public IPolicy Inside { get; set; }

        [JsonProperty("outside")]
        public IPolicy Outside { get; set; }

        public ConditionalGeoFencePolicy(
            IPolicy inside,
            IPolicy outside,
            bool? denyRootedJailbroken = false,
            bool? denyEmulatorSimulator = false,
            List<IFence> fences = null
            )
        {
            DenyRootedJailbroken = denyRootedJailbroken;
            DenyEmulatorSimulator = denyEmulatorSimulator;
            Fences = fences ?? new List<IFence>();
            Inside = inside;
            Outside = outside;
            Type = "COND_GEO";
        }

        public JoseJWTToken.Domain.Service.Policy.IPolicy FromTransport()
        {
            List<JoseJWTToken.Domain.Service.Policy.IFence> fences = new List<JoseJWTToken.Domain.Service.Policy.IFence>();
            foreach (IFence fence in Fences)
            {
                fences.Add(fence.FromTransport());
            }

            return new JoseJWTToken.Domain.Service.Policy.ConditionalGeoFencePolicy(
                    inside: Inside.FromTransport(),
                    outside: Outside.FromTransport(),
                    fences: fences,
                    denyRootedJailbroken: DenyRootedJailbroken,
                    denyEmulatorSimulator: DenyEmulatorSimulator
            );
        }
    }
}