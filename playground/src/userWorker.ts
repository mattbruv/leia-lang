import * as monaco from "monaco-editor";
//import LeiaWorker from "./leia.worker";
import editorWorker from "monaco-editor/esm/vs/editor/editor.worker?worker";
import { LeiaLang } from "./leiaLang";

// https://stackoverflow.com/a/68678328/2936448
monaco.languages.register({ id: "leia" });
monaco.languages.setMonarchTokensProvider("leia", LeiaLang);

// @ts-ignore
self.MonacoEnvironment = {
  getWorker(_: any, label: string) {
    if (label === "leia") {
      //return new LeiaWorker();
    }
    return new editorWorker();
  },
};
