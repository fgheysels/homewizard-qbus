using Fg.SolarProductionAlerter.Configuration;
using Microsoft.Extensions.Logging;

namespace Fg.SolarProductionAlerter.Qbus
{
    internal static class EqoWebSessionRegistry
    {
        private static EqoWebSession _currentSession;

        /// <summary>
        /// Gets an active QBus EqoWebSession.
        /// </summary>
        /// <remarks>Only initiates a new session when there's no existing session or the existing session is expired.</remarks>
        /// <param name="qbusSettings"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        internal static async Task<EqoWebSession> GetSessionAsync(QbusConfigurationSettings qbusSettings, ILogger logger)
        {
            if (_currentSession == null || _currentSession.IsExpired)
            {
                if (_currentSession != null)
                {
                    logger.LogDebug($"Current session is active for {_currentSession.SessionLifeTime} and is expired - creating new session");
                }

                _currentSession = await EqoWebSession.CreateSessionAsync(qbusSettings.IpAddress, qbusSettings.Port, qbusSettings.Username, qbusSettings.Password);
            }

            return _currentSession;
        }
    }
}
