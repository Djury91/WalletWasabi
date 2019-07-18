using System;
using System.Collections.Generic;
using System.Text;
using WalletWasabi.Helpers;

namespace System.Net
{
	public static class EndPointParser
	{
		public static string ToString(this EndPoint me, int defaultPort)
		{
			string host;
			int port;
			if (me is DnsEndPoint dnsEndPoint)
			{
				host = dnsEndPoint.Host;
				port = dnsEndPoint.Port;
			}
			else if (me is IPEndPoint ipEndPoint)
			{
				host = ipEndPoint.Address.ToString();
				port = ipEndPoint.Port;
			}
			else
			{
				throw new FormatException($"Invalid endpoint: {me.ToString()}");
			}

			if (port == 0)
			{
				port = defaultPort;
			}

			var endPointString = $"{host}:{port}";

			return endPointString;
		}

		public static bool TryParse(string endPointString, int defaultPort, out EndPoint endPoint)
		{
			endPoint = null;

			endPointString = Guard.Correct(endPointString);
			endPointString = endPointString.TrimEnd('/');
			endPointString = endPointString.TrimEnd(':');

			var lastIndex = endPointString.LastIndexOf(':');

			string portString = null;
			if (lastIndex != -1)
			{
				portString = endPointString.Substring(endPointString.LastIndexOf(':') + 1);
			}

			if (portString is null || !int.TryParse(portString, out int port))
			{
				port = defaultPort;
			}

			string host = endPointString.TrimEnd(portString, StringComparison.OrdinalIgnoreCase).TrimEnd(':');

			if (IPAddress.TryParse(host, out IPAddress addr))
			{
				endPoint = new IPEndPoint(addr, port);
			}
			else
			{
				try
				{
					endPoint = new DnsEndPoint(host, port);
				}
				catch
				{
					return false;
				}
			}

			return true;
		}
	}
}
