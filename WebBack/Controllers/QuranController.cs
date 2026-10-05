using Microsoft.AspNetCore.Mvc;
using WebBack.Services;

namespace WebBack.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuranController : ControllerBase
    {
        private readonly QuranApiService _quranApiService;

        public QuranController(QuranApiService quranApiService)
        {
            _quranApiService = quranApiService;
        }

        // 1. Surah List: GET /api/quran/surahs
        [HttpGet("surahs")]
        public async Task<IActionResult> GetAllSurahs()
        {
            try
            {
                var data = await _quranApiService.GetQuranDataAsync("chapters");
                return Content(data, "application/json");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // 2. Single Surah Info: GET /api/quran/surah/1
        [HttpGet("surah/{id}")]
        public async Task<IActionResult> GetSurah(int id)
        {
            try
            {
                var data = await _quranApiService.GetQuranDataAsync($"chapters/{id}");
                return Content(data, "application/json");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // 3. Single Ayah by Key (e.g. 1:1, 2:255): GET /api/quran/verse/1/1
        [HttpGet("verse/{chapter}/{verse}")]
        public async Task<IActionResult> GetAyahByKey(
            int chapter,
            int verse,
            [FromQuery] string language = "en",
            [FromQuery] bool words = true)
        {
            try
            {
                var verseKey = $"{chapter}:{verse}";
                var endpoint = $"verses/by_key/{verseKey}?language={language}&words={words.ToString().ToLower()}&fields=text_uthmani";
                var data = await _quranApiService.GetQuranDataAsync(endpoint);
                return Content(data, "application/json");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // 4. Surah Verses with Arabic, Translation & Word-by-word: GET /api/quran/surah/1/verses
        [HttpGet("surah/{id}/verses")]
        public async Task<IActionResult> GetSurahVerses(
            int id,
            [FromQuery] string language = "bn",
            [FromQuery] string? translations = null,
            [FromQuery] bool words = false)
        {
            try
            {
                // translations ID pass korle oita specific bhabe ashbe, na thakle default language translation ashbe
                var translationParam = string.IsNullOrEmpty(translations) ? $"language={language}" : $"translations={translations}";
                var endpoint = $"verses/by_chapter/{id}?{translationParam}&words={words.ToString().ToLower()}&fields=text_uthmani";

                var data = await _quranApiService.GetQuranDataAsync(endpoint);
                return Content(data, "application/json");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // 5. Available Translations List: GET /api/quran/translations
        [HttpGet("translations")]
        public async Task<IActionResult> GetTranslations()
        {
            try
            {
                var data = await _quranApiService.GetQuranDataAsync("resources/translations");
                return Content(data, "application/json");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // 6. Available Languages List: GET /api/quran/languages
        [HttpGet("languages")]
        public async Task<IActionResult> GetLanguages()
        {
            try
            {
                var data = await _quranApiService.GetQuranDataAsync("resources/languages");
                return Content(data, "application/json");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // 7. Verses by Juz: GET /api/quran/juz/1
        [HttpGet("juz/{juzNumber}")]
        public async Task<IActionResult> GetVersesByJuz(int juzNumber)
        {
            try
            {
                var data = await _quranApiService.GetQuranDataAsync($"verses/by_juz/{juzNumber}?fields=text_uthmani");
                return Content(data, "application/json");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // 8. Verses by Page: GET /api/quran/page/1
        [HttpGet("page/{pageNumber}")]
        public async Task<IActionResult> GetVersesByPage(int pageNumber)
        {
            try
            {
                var data = await _quranApiService.GetQuranDataAsync($"verses/by_page/{pageNumber}?fields=text_uthmani");
                return Content(data, "application/json");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // 9. Verses by Hizb: GET /api/quran/hizb/1
        [HttpGet("hizb/{hizbNumber}")]
        public async Task<IActionResult> GetVersesByHizb(int hizbNumber)
        {
            try
            {
                var data = await _quranApiService.GetQuranDataAsync($"verses/by_hizb/{hizbNumber}?fields=text_uthmani");
                return Content(data, "application/json");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // 10. Verses by Manzil: GET /api/quran/manzil/1
        [HttpGet("manzil/{manzilNumber}")]
        public async Task<IActionResult> GetVersesByManzil(int manzilNumber)
        {
            try
            {
                var data = await _quranApiService.GetQuranDataAsync($"verses/by_manzil/{manzilNumber}?fields=text_uthmani");
                return Content(data, "application/json");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // 11. Verses by Ruku: GET /api/quran/ruku/1
        [HttpGet("ruku/{rukuNumber}")]
        public async Task<IActionResult> GetVersesByRuku(int rukuNumber)
        {
            try
            {
                var data = await _quranApiService.GetQuranDataAsync($"verses/by_ruku/{rukuNumber}?fields=text_uthmani");
                return Content(data, "application/json");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // 12. Tafsir Resources List: GET /api/quran/tafsirs
        [HttpGet("tafsirs")]
        public async Task<IActionResult> GetTafsirs()
        {
            try
            {
                var data = await _quranApiService.GetQuranDataAsync("resources/tafsirs");
                return Content(data, "application/json");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // 13. Ayah Tafsir by Key: GET /api/quran/tafsir/169/1/1
        [HttpGet("tafsir/{tafsirId}/{chapter}/{verse}")]
        public async Task<IActionResult> GetAyahTafsir(int tafsirId, int chapter, int verse)
        {
            try
            {
                var verseKey = $"{chapter}:{verse}";
                var data = await _quranApiService.GetQuranDataAsync($"tafsirs/{tafsirId}/by_ayah/{verseKey}");
                return Content(data, "application/json");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // 14. Audio Reciters List: GET /api/quran/reciters
        [HttpGet("reciters")]
        public async Task<IActionResult> GetReciters()
        {
            try
            {
                var data = await _quranApiService.GetQuranDataAsync("resources/recitations");
                return Content(data, "application/json");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // 15. Surah Audio Recitation: GET /api/quran/surah/1/audio
        [HttpGet("surah/{id}/audio")]
        public async Task<IActionResult> GetSurahAudio(int id, [FromQuery] int reciterId = 7)
        {
            try
            {
                var data = await _quranApiService.GetQuranDataAsync($"chapter_recitations/{reciterId}/{id}");
                return Content(data, "application/json");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
        // ১৬. কুরআন সার্চ: GET /api/quran/search?q=জান্নাত&language=bn
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string q, [FromQuery] string language = "bn", [FromQuery] int page = 1)
        {
            try
            {
                var data = await _quranApiService.GetQuranDataAsync($"search?q={Uri.EscapeDataString(q)}&language={language}&page={page}");
                return Content(data, "application/json");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // ১৭. সূরার বিস্তারিত পটভূমি/তথ্য: GET /api/quran/surah/1/info
        [HttpGet("surah/{id}/info")]
        public async Task<IActionResult> GetSurahInfo(int id, [FromQuery] string language = "bn")
        {
            try
            {
                var data = await _quranApiService.GetQuranDataAsync($"chapters/{id}/info?language={language}");
                return Content(data, "application/json");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // ১৮. র্যান্ডম আয়াত (Daily Ayah): GET /api/quran/verse/random
        [HttpGet("verse/random")]
        public async Task<IActionResult> GetRandomVerse([FromQuery] string language = "bn")
        {
            try
            {
                var data = await _quranApiService.GetQuranDataAsync($"verses/random?language={language}&fields=text_uthmani");
                return Content(data, "application/json");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // ১৯. একক আয়াতের অডিও: GET /api/quran/verse/1/1/audio
        [HttpGet("verse/{chapter}/{verse}/audio")]
        public async Task<IActionResult> GetAyahAudio(int chapter, int verse, [FromQuery] int reciterId = 7)
        {
            try
            {
                var verseKey = $"{chapter}:{verse}";
                var data = await _quranApiService.GetQuranDataAsync($"recitations/{reciterId}/by_ayah/{verseKey}");
                return Content(data, "application/json");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // ২০. অনুবাদের ফুটনোট/টিকা: GET /api/quran/footnote/1234
        [HttpGet("footnote/{id}")]
        public async Task<IActionResult> GetFootNote(int id)
        {
            try
            {
                var data = await _quranApiService.GetQuranDataAsync($"foot_notes/{id}");
                return Content(data, "application/json");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}