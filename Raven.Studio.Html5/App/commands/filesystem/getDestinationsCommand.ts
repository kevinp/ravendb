﻿import commandBase = require("commands/commandBase");
import filesystem = require("models/filesystem/filesystem");
import getConfigurationByKeyCommand = require("commands/filesystem/getConfigurationByKeyCommand");

class getFilesystemDestinationsCommand extends commandBase {

    constructor(private fs: filesystem) {
        super();
    }

    execute(): JQueryPromise<string[]> {

        var url = "/config";
        var args = {
            name: "Raven/Synchronization/Destinations",
        };

        var task = $.Deferred();
        this.query<Array<Pair<string, string[]>>>(url, args, this.fs)
            .done(data => {                
                if (data.hasOwnProperty('url'))
                    task.resolve(data['url']);
                else
                    task.resolve({});
            });

        return task;
    }
}

export = getFilesystemDestinationsCommand;