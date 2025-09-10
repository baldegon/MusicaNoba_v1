using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MusicaNoba_v1.Services
{

    public interface ISongstatsService
    {
        Task<JsonDocument> GetArtistInfoAsync(
            string? spotifyArtistId = null,
            string? songstatsArtistId = null,
            CancellationToken ct = default);
    }

    public class SongstatsService : ISongstatsService
    {
        private readonly IHttpClientFactory _httpFactory;
        public SongstatsService(IHttpClientFactory httpFactory) => _httpFactory = httpFactory;

        public async Task<JsonDocument> GetArtistInfoAsync(
            string? spotifyArtistId = null,
            string? songstatsArtistId = null,
            CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(spotifyArtistId) && string.IsNullOrWhiteSpace(songstatsArtistId))
                throw new ArgumentException("Es necesario enviar spotify_artist_id o songstats_artist_id");

            var client = _httpFactory.CreateClient("SongstatsRapidApi");

            var qs = new List<string>(2);
            if(!string.IsNullOrWhiteSpace(spotifyArtistId))
                qs.Add($"spotify_artist_id={Uri.EscapeDataString(spotifyArtistId)}");
            if(!string.IsNullOrWhiteSpace(songstatsArtistId))
                qs.Add($"songstats_artst_id={Uri.EscapeDataString(songstatsArtistId)}");

            var url = $"artists/info?{string.Join("&", qs)}";

            using var resp = await client.GetAsync(url, ct);
            resp.EnsureSuccessStatusCode();

            await using var s = await resp.Content.ReadAsStreamAsync(ct);
            return await JsonDocument.ParseAsync(s, cancellationToken: ct);


        }


    }
}
