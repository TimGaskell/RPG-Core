using System.Collections.Generic;

namespace RPG.Stats {

    public interface IModifierProvider {

        IEnumerable<float> GetAdditiveModifer(Stat stat);
    }
}