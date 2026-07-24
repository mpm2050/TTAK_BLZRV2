using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.JSInterop;

namespace TTAK_BLZRV2.Services
{

    public class SqlJsService
    {
        private readonly IJSRuntime _js;
        private bool _init;

        public SqlJsService(IJSRuntime js)
        {
            _js = js;
        }

        public async Task InitAsync()
        {
            if (_init)
            {
                return;
            }

            await _js.InvokeVoidAsync("sqliteJs.init");
            _init = true;
        }

        public Task<string> OpenAsync(byte[]? bytes = null)
        {
            return _js.InvokeAsync<string>("sqliteJs.openDb", bytes).AsTask();
        }

        public async Task<List<Dictionary<string, object>>> QueryAsync(
            string id,
            string sql,
            object[]? args = null)
        {
            var raw = await _js.InvokeAsync<object>("sqliteJs.exec", id, sql, args);

            // Convert JS objects -> JSON -> Dictionary<string, object>
            return JsonSerializer.Deserialize<List<Dictionary<string, object>>>(
                       JsonSerializer.Serialize(raw))
                   ?? new List<Dictionary<string, object>>();
        }

        public Task<byte[]> ExportAsync(string id)
        {
            return _js.InvokeAsync<byte[]>("sqliteJs.export", id).AsTask();
        }

        public Task CloseAsync(string id)
        {
            return _js.InvokeVoidAsync("sqliteJs.close", id).AsTask();
        }
    }
}
