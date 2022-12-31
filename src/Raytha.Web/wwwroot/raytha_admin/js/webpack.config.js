const path = require("path");

module.exports = (env, argv) =>
{
    const isDevBuild = !(argv.mode && argv.mode == "production");

    let config = {
        entry: "./src/application.js",
        output:
        {
            filename: "main.js",
            path: path.resolve(__dirname, "dist"),
        },
        module:
        {
            rules: [
            {
                test: /\.m?js$/,
                exclude: /(node_modules|bower_components)/,
                use:
                {
                    loader: "babel-loader",
                    options:
                    {
                        presets: [
                            ["@babel/preset-env", { shippedProposals: true }]
                        ],
                    },
                },
            }, ],
        }
    };

    if (isDevBuild);
        config.devtool = "source-map";

    return config;
};