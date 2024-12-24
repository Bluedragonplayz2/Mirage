namespace Mir_Utilities;

public class SessionApi
{
     public static async Task<List<SessionApiSchema.GetSessionSnapshot>?> GetSessionSnapshot(ApiCaller caller)
     {
          dynamic sessionList = await caller.GetApi("sessions");
          if (sessionList.Count == 0)
          {
               return null;
          }

          List<SessionApiSchema.GetSessionSnapshot> sessionListSnapshot = new List<SessionApiSchema.GetSessionSnapshot>();
          foreach (var session in sessionList)
          {
               SessionApiSchema.GetSessionSnapshot sessionSnapshot = new SessionApiSchema.GetSessionSnapshot();
               sessionSnapshot.Name = (session.name != null) ? session.name : "Unnamed Session";
               sessionSnapshot.Guid = (session.guid != null) ? session.guid : "Unknown Guid";
               sessionListSnapshot.Add(sessionSnapshot);
          }

          return sessionListSnapshot;
     }
     public static async Task<SessionApiSchema.GetSessionByGuidSnapshot> GetSessionByGuid(ApiCaller caller, String guid)
     {
          dynamic session = await caller.GetApi($"sessions/{guid}");
          SessionApiSchema.GetSessionByGuidSnapshot sessionSnapshot = new SessionApiSchema.GetSessionByGuidSnapshot();
          sessionSnapshot.Guid = session.guid!;
          sessionSnapshot.Name = session.name!;
          sessionSnapshot.Description = session.description!;
          return sessionSnapshot;
     }
     
     public static async Task<String> PostSession(ApiCaller caller, String guid, String name, String description)
     {
          dynamic session = new
          {
               guid,
               name,
               description
          };
          dynamic response = await caller.PostApi("sessions", session);
          return response.guid!;
     }
     
     public static async Task<String> PutSession(ApiCaller caller, String guid, String name, String description)
     {
          dynamic session = new
          {
               name,
               description
          };
          dynamic response = await caller.PutApi($"sessions/{guid}", session);
          return response.guid!;
     }
     public static async Task<string> DeleteSession(ApiCaller caller, String guid)
     { 
          await caller.DeleteApi($"sessions/{guid}");
          return guid;
     }
     
     public static async Task<byte[]> SessionExport(ApiCaller caller, String guid)
     {
          //Todo: Make sure this works
          
          dynamic response = await caller.GetRawApi($"sessions/{guid}/export");

          return response;
     }

     public static async Task<SessionApiSchema.GetActiveSessionImportSnapshot>  SessionImport(ApiCaller caller, byte[] file)
     {
          //Todo: Make sure this works
          
          dynamic response = await caller.PostFileApi("sessions/import", "file",file, "newSite.site");
          SessionApiSchema.GetActiveSessionImportSnapshot snapshot = new SessionApiSchema.GetActiveSessionImportSnapshot();
          snapshot.Status = response.status!;
          snapshot.SessionsTotal = response.sessions_total!;
          snapshot.SessionsImported = response.sessions_imported!;
          snapshot.ErrorMessage = response.error_message!;
          return snapshot;
     }
     public static async Task<SessionApiSchema.GetActiveSessionImportSnapshot> GetActiveSessionImportSnapshot(ApiCaller caller)
     {
          //experimental 
          dynamic response = await caller.GetApi("sessions/import");
          SessionApiSchema.GetActiveSessionImportSnapshot snapshot = new SessionApiSchema.GetActiveSessionImportSnapshot();
          snapshot.Status = response.status!;
          snapshot.SessionsTotal = response.sessions_total!;
          snapshot.SessionsImported = response.sessions_imported!;
          snapshot.ErrorMessage = response.error_message!;
          return snapshot;
     }
}