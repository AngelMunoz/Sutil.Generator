const hljs = require('highlight.js');
const fs = require('fs/promises');

/**
 * 
 * @param {import('snowpack').SnowpackConfig} snowpackConfig 
 * @param {Record<string, any> | undefined | null} pluginOptions 
 * @returns {import('snowpack').SnowpackPlugin}
 */
function markdownPlugin(snowpackConfig, pluginOptions) {
    const Md = require('markdown-it')({
        breaks: true,
        typographer: true,
        linkify: true,
        html: true,
        highlight(str, lang) {
            if (lang && hljs.getLanguage(lang)) {
                try {
                    return hljs.highlight(str, { language: lang }).value;
                } catch (__) { }
            }
            return ''; // use external default escaping
        },
        ...(pluginOptions && pluginOptions)
    });

    return {
        resolve: {
            input: [".md"],
            output: [".html"]
        },
        async load({ filePath }) {
            const content = await fs.readFile(filePath, { encoding: 'utf8' });
            return Md.render(content);
        }
    };

}


module.exports = markdownPlugin;