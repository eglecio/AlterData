/* eslint-env node */

/*
 * This file runs in a Node context (it's NOT transpiled by Babel), so use only
 * the ES6 features that are supported by your Node version. https://node.green/
 */

// Configuration for your app
// https://v2.quasar.dev/quasar-cli-vite/quasar-config-js


const { configure } = require('quasar/wrappers');
const path = require('path');

module.exports = configure(function (ctx) {
  return {


    // https://v2.quasar.dev/quasar-cli/prefetch-feature
    // preFetch: true,

    // app boot file (/src/boot)
    // --> boot files are part of "main.js"
    // https://v2.quasar.dev/quasar-cli/boot-files
    boot: [
      'i18n',
      'axios',
    ],

    // https://v2.quasar.dev/quasar-cli-vite/quasar-config-js#css
    css: [
      'app.css'
    ],

    // https://github.com/quasarframework/quasar/tree/dev/extras
    extras: [
      // 'ionicons-v4',
      // 'mdi-v5',
      'fontawesome-v6',
      // 'eva-icons',
      // 'themify',
      // 'line-awesome',
      // 'roboto-font-latin-ext', // this or either 'roboto-font', NEVER both!

      'roboto-font', // optional, you are not bound to it
      'material-icons', // optional, you are not bound to it
    ],

    // Full list of options: https://v2.quasar.dev/quasar-cli-vite/quasar-config-js#build
    build: {
      env: ctx.dev ? {
        API: JSON.stringify('https://localhost:7188')
      } : {
        API: JSON.stringify('https://localhost:7188')// Mudar para o endereco do WS online...
      },

      target: {
        browser: [ 'es2019', 'edge88', 'firefox78', 'chrome87', 'safari13.1' ],
        node: 'node16'
      },

      vueRouterMode: 'history', // available values: 'hash', 'history'
      // vueRouterBase,
      // vueDevtools,
      // vueOptionsAPI: false,

      // rebuildCache: true, // rebuilds Vite/linter/etc cache on startup

      // publicPath: '/',
      // analyze: true,
      // env: {},
      // rawDefine: {}
      // ignorePublicFolder: true,
      // minify: false,
      // polyfillModulePreload: true,
      // distDir

      // extendViteConf (viteConf) {},
      // viteVuePluginOptions: {},

      vitePlugins: [
        ['@intlify/vite-plugin-vue-i18n', {
          // if you want to use Vue I18n Legacy API, you need to set `compositionOnly: false`
          // compositionOnly: false,

          // you need to set i18n resource including paths !
          include: path.resolve(__dirname, './src/i18n/**')
        }]
      ]
    },

    // Full list of options: https://v2.quasar.dev/quasar-cli-vite/quasar-config-js#devServer
    devServer: {
      // https: true
      open: true // opens browser window automatically
    },

    // https://v2.quasar.dev/quasar-cli-vite/quasar-config-js#framework
    framework: {
      config: {},

      // iconSet: 'material-icons', // Quasar icon set
      // lang: 'en-US', // Quasar language pack

      // For special cases outside of where the auto-import strategy can have an impact
      // (like functional components as one of the examples),
      // you can manually specify Quasar components/directives to be available everywhere:
      //
      // components: [],
      // directives: [],

      // Quasar plugins
      plugins: [
        'AppFullscreen',
        'LocalStorage',
        'SessionStorage',
        'Notify',
        'Loading'
      ]
    },

    // animations: 'all', // --- includes all animations
    // https://v2.quasar.dev/options/animations
    animations: [],

    // https://v2.quasar.dev/quasar-cli-vite/quasar-config-js#property-sourcefiles
    // sourceFiles: {
    //   rootComponent: 'src/App.vue',
    //   router: 'src/router/index',
    //   store: 'src/store/index',
    //   registerServiceWorker: 'src-pwa/register-service-worker',
    //   serviceWorker: 'src-pwa/custom-service-worker',
    //   pwaManifestFile: 'src-pwa/manifest.json',
    //   electronMain: 'src-electron/electron-main',
    //   electronPreload: 'src-electron/electron-preload'
    // },

    // https://v2.quasar.dev/quasar-cli/developing-ssr/configuring-ssr
    ssr: {
      // ssrPwaHtmlFilename: 'offline.html', // do NOT use index.html as name!
                                          // will mess up SSR

      // extendSSRWebserverConf (esbuildConf) {},
      // extendPackageJson (json) {},

      pwa: false,

      // manualStoreHydration: true,
      // manualPostHydrationTrigger: true,

      prodPort: 3000, // The default port that the production server should use
                      // (gets superseded if process.env.PORT is specified at runtime)

      middlewares: [
        'render' // keep this as last one
      ]
    },

    // https://v2.quasar.dev/quasar-cli/developing-pwa/configuring-pwa
    pwa: {
      workboxMode: 'generateSW', // or 'injectManifest'
      injectPwaMetaTags: true,
      swFilename: 'sw.js',
      manifestFilename: 'manifest.json',
      useCredentialsForManifestTag: false,
      // useFilenameHashes: true,
      // extendGenerateSWOptions (cfg) {}
      // extendInjectManifestOptions (cfg) {},
      extendManifestJson (json) {
        json.name = 'Admin Alterdata'
        json.short_name = 'Admin'
        json.description = 'Admin para gestão Alterdata'
        json.icons = [
          {
            "src": "icons/windows11/SmallTile.scale-100.png",
            "sizes": "71x71"
          },
          {
            "src": "icons/windows11/SmallTile.scale-125.png",
            "sizes": "89x89"
          },
          {
            "src": "icons/windows11/SmallTile.scale-150.png",
            "sizes": "107x107"
          },
          {
            "src": "icons/windows11/SmallTile.scale-200.png",
            "sizes": "142x142"
          },
          {
            "src": "icons/windows11/SmallTile.scale-400.png",
            "sizes": "284x284"
          },
          {
            "src": "icons/windows11/Square150x150Logo.scale-100.png",
            "sizes": "150x150"
          },
          {
            "src": "icons/windows11/Square150x150Logo.scale-125.png",
            "sizes": "188x188"
          },
          {
            "src": "icons/windows11/Square150x150Logo.scale-150.png",
            "sizes": "225x225"
          },
          {
            "src": "icons/windows11/Square150x150Logo.scale-200.png",
            "sizes": "300x300"
          },
          {
            "src": "icons/windows11/Square150x150Logo.scale-400.png",
            "sizes": "600x600"
          },
          {
            "src": "icons/windows11/Wide310x150Logo.scale-100.png",
            "sizes": "310x150"
          },
          {
            "src": "icons/windows11/Wide310x150Logo.scale-125.png",
            "sizes": "388x188"
          },
          {
            "src": "icons/windows11/Wide310x150Logo.scale-150.png",
            "sizes": "465x225"
          },
          {
            "src": "icons/windows11/Wide310x150Logo.scale-200.png",
            "sizes": "620x300"
          },
          {
            "src": "icons/windows11/Wide310x150Logo.scale-400.png",
            "sizes": "1240x600"
          },
          {
            "src": "icons/windows11/LargeTile.scale-100.png",
            "sizes": "310x310"
          },
          {
            "src": "icons/windows11/LargeTile.scale-125.png",
            "sizes": "388x388"
          },
          {
            "src": "icons/windows11/LargeTile.scale-150.png",
            "sizes": "465x465"
          },
          {
            "src": "icons/windows11/LargeTile.scale-200.png",
            "sizes": "620x620"
          },
          {
            "src": "icons/windows11/LargeTile.scale-400.png",
            "sizes": "1240x1240"
          },
          {
            "src": "icons/windows11/Square44x44Logo.scale-100.png",
            "sizes": "44x44"
          },
          {
            "src": "icons/windows11/Square44x44Logo.scale-125.png",
            "sizes": "55x55"
          },
          {
            "src": "icons/windows11/Square44x44Logo.scale-150.png",
            "sizes": "66x66"
          },
          {
            "src": "icons/windows11/Square44x44Logo.scale-200.png",
            "sizes": "88x88"
          },
          {
            "src": "icons/windows11/Square44x44Logo.scale-400.png",
            "sizes": "176x176"
          },
          {
            "src": "icons/windows11/StoreLogo.scale-100.png",
            "sizes": "50x50"
          },
          {
            "src": "icons/windows11/StoreLogo.scale-125.png",
            "sizes": "63x63"
          },
          {
            "src": "icons/windows11/StoreLogo.scale-150.png",
            "sizes": "75x75"
          },
          {
            "src": "icons/windows11/StoreLogo.scale-200.png",
            "sizes": "100x100"
          },
          {
            "src": "icons/windows11/StoreLogo.scale-400.png",
            "sizes": "200x200"
          },
          {
            "src": "icons/windows11/SplashScreen.scale-100.png",
            "sizes": "620x300"
          },
          {
            "src": "icons/windows11/SplashScreen.scale-125.png",
            "sizes": "775x375"
          },
          {
            "src": "icons/windows11/SplashScreen.scale-150.png",
            "sizes": "930x450"
          },
          {
            "src": "icons/windows11/SplashScreen.scale-200.png",
            "sizes": "1240x600"
          },
          {
            "src": "icons/windows11/SplashScreen.scale-400.png",
            "sizes": "2480x1200"
          },
          {
            "src": "icons/windows11/Square44x44Logo.targetsize-16.png",
            "sizes": "16x16"
          },
          {
            "src": "icons/windows11/Square44x44Logo.targetsize-20.png",
            "sizes": "20x20"
          },
          {
            "src": "icons/windows11/Square44x44Logo.targetsize-24.png",
            "sizes": "24x24"
          },
          {
            "src": "icons/windows11/Square44x44Logo.targetsize-30.png",
            "sizes": "30x30"
          },
          {
            "src": "icons/windows11/Square44x44Logo.targetsize-32.png",
            "sizes": "32x32"
          },
          {
            "src": "icons/windows11/Square44x44Logo.targetsize-36.png",
            "sizes": "36x36"
          },
          {
            "src": "icons/windows11/Square44x44Logo.targetsize-40.png",
            "sizes": "40x40"
          },
          {
            "src": "icons/windows11/Square44x44Logo.targetsize-44.png",
            "sizes": "44x44"
          },
          {
            "src": "icons/windows11/Square44x44Logo.targetsize-48.png",
            "sizes": "48x48"
          },
          {
            "src": "icons/windows11/Square44x44Logo.targetsize-60.png",
            "sizes": "60x60"
          },
          {
            "src": "icons/windows11/Square44x44Logo.targetsize-64.png",
            "sizes": "64x64"
          },
          {
            "src": "icons/windows11/Square44x44Logo.targetsize-72.png",
            "sizes": "72x72"
          },
          {
            "src": "icons/windows11/Square44x44Logo.targetsize-80.png",
            "sizes": "80x80"
          },
          {
            "src": "icons/windows11/Square44x44Logo.targetsize-96.png",
            "sizes": "96x96"
          },
          {
            "src": "icons/windows11/Square44x44Logo.targetsize-256.png",
            "sizes": "256x256"
          },
          {
            "src": "icons/windows11/Square44x44Logo.altform-unplated_targetsize-16.png",
            "sizes": "16x16"
          },
          {
            "src": "icons/windows11/Square44x44Logo.altform-unplated_targetsize-20.png",
            "sizes": "20x20"
          },
          {
            "src": "icons/windows11/Square44x44Logo.altform-unplated_targetsize-24.png",
            "sizes": "24x24"
          },
          {
            "src": "icons/windows11/Square44x44Logo.altform-unplated_targetsize-30.png",
            "sizes": "30x30"
          },
          {
            "src": "icons/windows11/Square44x44Logo.altform-unplated_targetsize-32.png",
            "sizes": "32x32"
          },
          {
            "src": "icons/windows11/Square44x44Logo.altform-unplated_targetsize-36.png",
            "sizes": "36x36"
          },
          {
            "src": "icons/windows11/Square44x44Logo.altform-unplated_targetsize-40.png",
            "sizes": "40x40"
          },
          {
            "src": "icons/windows11/Square44x44Logo.altform-unplated_targetsize-44.png",
            "sizes": "44x44"
          },
          {
            "src": "icons/windows11/Square44x44Logo.altform-unplated_targetsize-48.png",
            "sizes": "48x48"
          },
          {
            "src": "icons/windows11/Square44x44Logo.altform-unplated_targetsize-60.png",
            "sizes": "60x60"
          },
          {
            "src": "icons/windows11/Square44x44Logo.altform-unplated_targetsize-64.png",
            "sizes": "64x64"
          },
          {
            "src": "icons/windows11/Square44x44Logo.altform-unplated_targetsize-72.png",
            "sizes": "72x72"
          },
          {
            "src": "icons/windows11/Square44x44Logo.altform-unplated_targetsize-80.png",
            "sizes": "80x80"
          },
          {
            "src": "icons/windows11/Square44x44Logo.altform-unplated_targetsize-96.png",
            "sizes": "96x96"
          },
          {
            "src": "icons/windows11/Square44x44Logo.altform-unplated_targetsize-256.png",
            "sizes": "256x256"
          },
          {
            "src": "icons/windows11/Square44x44Logo.altform-lightunplated_targetsize-16.png",
            "sizes": "16x16"
          },
          {
            "src": "icons/windows11/Square44x44Logo.altform-lightunplated_targetsize-20.png",
            "sizes": "20x20"
          },
          {
            "src": "icons/windows11/Square44x44Logo.altform-lightunplated_targetsize-24.png",
            "sizes": "24x24"
          },
          {
            "src": "icons/windows11/Square44x44Logo.altform-lightunplated_targetsize-30.png",
            "sizes": "30x30"
          },
          {
            "src": "icons/windows11/Square44x44Logo.altform-lightunplated_targetsize-32.png",
            "sizes": "32x32"
          },
          {
            "src": "icons/windows11/Square44x44Logo.altform-lightunplated_targetsize-36.png",
            "sizes": "36x36"
          },
          {
            "src": "icons/windows11/Square44x44Logo.altform-lightunplated_targetsize-40.png",
            "sizes": "40x40"
          },
          {
            "src": "icons/windows11/Square44x44Logo.altform-lightunplated_targetsize-44.png",
            "sizes": "44x44"
          },
          {
            "src": "icons/windows11/Square44x44Logo.altform-lightunplated_targetsize-48.png",
            "sizes": "48x48"
          },
          {
            "src": "icons/windows11/Square44x44Logo.altform-lightunplated_targetsize-60.png",
            "sizes": "60x60"
          },
          {
            "src": "icons/windows11/Square44x44Logo.altform-lightunplated_targetsize-64.png",
            "sizes": "64x64"
          },
          {
            "src": "icons/windows11/Square44x44Logo.altform-lightunplated_targetsize-72.png",
            "sizes": "72x72"
          },
          {
            "src": "icons/windows11/Square44x44Logo.altform-lightunplated_targetsize-80.png",
            "sizes": "80x80"
          },
          {
            "src": "icons/windows11/Square44x44Logo.altform-lightunplated_targetsize-96.png",
            "sizes": "96x96"
          },
          {
            "src": "icons/windows11/Square44x44Logo.altform-lightunplated_targetsize-256.png",
            "sizes": "256x256"
          },
          {
            "src": "icons/android/android-launchericon-512-512.png",
            "sizes": "512x512"
          },
          {
            "src": "icons/android/android-launchericon-192-192.png",
            "sizes": "192x192"
          },
          {
            "src": "icons/android/android-launchericon-144-144.png",
            "sizes": "144x144"
          },
          {
            "src": "icons/android/android-launchericon-96-96.png",
            "sizes": "96x96"
          },
          {
            "src": "icons/android/android-launchericon-72-72.png",
            "sizes": "72x72"
          },
          {
            "src": "icons/android/android-launchericon-48-48.png",
            "sizes": "48x48"
          },
          {
            "src": "icons/ios/16.png",
            "sizes": "16x16"
          },
          {
            "src": "icons/ios/20.png",
            "sizes": "20x20"
          },
          {
            "src": "icons/ios/29.png",
            "sizes": "29x29"
          },
          {
            "src": "icons/ios/32.png",
            "sizes": "32x32"
          },
          {
            "src": "icons/ios/40.png",
            "sizes": "40x40"
          },
          {
            "src": "icons/ios/50.png",
            "sizes": "50x50"
          },
          {
            "src": "icons/ios/57.png",
            "sizes": "57x57"
          },
          {
            "src": "icons/ios/58.png",
            "sizes": "58x58"
          },
          {
            "src": "icons/ios/60.png",
            "sizes": "60x60"
          },
          {
            "src": "icons/ios/64.png",
            "sizes": "64x64"
          },
          {
            "src": "icons/ios/72.png",
            "sizes": "72x72"
          },
          {
            "src": "icons/ios/76.png",
            "sizes": "76x76"
          },
          {
            "src": "icons/ios/80.png",
            "sizes": "80x80"
          },
          {
            "src": "icons/ios/87.png",
            "sizes": "87x87"
          },
          {
            "src": "icons/ios/100.png",
            "sizes": "100x100"
          },
          {
            "src": "icons/ios/114.png",
            "sizes": "114x114"
          },
          {
            "src": "icons/ios/120.png",
            "sizes": "120x120"
          },
          {
            "src": "icons/ios/128.png",
            "sizes": "128x128"
          },
          {
            "src": "icons/ios/144.png",
            "sizes": "144x144"
          },
          {
            "src": "icons/ios/152.png",
            "sizes": "152x152"
          },
          {
            "src": "icons/ios/167.png",
            "sizes": "167x167"
          },
          {
            "src": "icons/ios/180.png",
            "sizes": "180x180"
          },
          {
            "src": "icons/ios/192.png",
            "sizes": "192x192"
          },
          {
            "src": "icons/ios/256.png",
            "sizes": "256x256"
          },
          {
            "src": "icons/ios/512.png",
            "sizes": "512x512"
          },
          {
            "src": "icons/ios/1024.png",
            "sizes": "1024x1024"
          }
        ]
        // json.background_color = '#ffffff'
        // theme_color: '#027be3'
        json.theme_color = '#96D032'

      },
      // extendPWACustomSWConf (esbuildConf) {}
    },

    // Full list of options: https://v2.quasar.dev/quasar-cli/developing-cordova-apps/configuring-cordova
    cordova: {
      // noIosLegacyBuildFlag: true, // uncomment only if you know what you are doing
    },

    // Full list of options: https://v2.quasar.dev/quasar-cli/developing-capacitor-apps/configuring-capacitor
    capacitor: {
      hideSplashscreen: true
    },

    // Full list of options: https://v2.quasar.dev/quasar-cli/developing-electron-apps/configuring-electron
    electron: {
      // extendElectronMainConf (esbuildConf)
      // extendElectronPreloadConf (esbuildConf)

      inspectPort: 5858,

      bundler: 'packager', // 'packager' or 'builder'

      packager: {
        // https://github.com/electron-userland/electron-packager/blob/master/docs/api.md#options

        // OS X / Mac App Store
        // appBundleId: '',
        // appCategoryType: '',
        // osxSign: '',
        // protocol: 'myapp://path',

        // Windows only
        // win32metadata: { ... }
      },

      builder: {
        // https://www.electron.build/configuration/configuration

        appId: 'quasar-admin2'
      }
    },

    // Full list of options: https://v2.quasar.dev/quasar-cli-vite/developing-browser-extensions/configuring-bex
    bex: {
      contentScripts: [
        'my-content-script'
      ],

      // extendBexScriptsConf (esbuildConf) {}
      // extendBexManifestJson (json) {}
    }
  }
});
