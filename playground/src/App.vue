<template>
  <div>
    <Dropdown @change="onProgramChange" v-model:model-value="selection" option-label="name" :options="LEIA_EXAMPLES" />
  </div>
  <div class="playground">
    <Panel header="Editor" class="section">
      <MonacoEditor :code="sourceCode" @update:code="onCodeUpdate" />
    </Panel>

    <Panel header="Output" class="section output-panel">
      <pre>{{ asmOutput }}</pre>
    </Panel>

    <Panel header="Output" class="section output-panel">
      <button @click="onRun">Run</button>
      <pre>{{ programOutput }}</pre>
    </Panel>
  </div>
</template>

<script lang="ts" setup>
//@ts-ignore
import { compileWeb } from "../../Compiler/CompilerLib/Compiler.fs.js"
import Dropdown from "primevue/dropdown";
import { onMounted, ref } from 'vue'
import MonacoEditor from "./components/MonacoEditor.vue"
import { run_asm } from "vm";
import { LEIA_EXAMPLES } from "./leiaLang.js";
import type { SelectChangeEvent } from "primevue/select";

const sourceCode = ref<string>(LEIA_EXAMPLES[0].source)
const asmOutput = ref<string>('')
const programOutput = ref<string>('')

const selection = ref<{
  name: string, source: string
} | null>(LEIA_EXAMPLES[0])

function onProgramChange(event: SelectChangeEvent) {
  sourceCode.value = event.value.source
  asmOutput.value = compileToAsm(sourceCode.value)
  onRun()
}

onMounted(() => {
  asmOutput.value = compileToAsm(sourceCode.value)
  onRun()
})

function onCodeUpdate(newCode: string) {
  sourceCode.value = newCode
  asmOutput.value = compileToAsm(newCode)
}

function onRun() {
  if (asmOutput.value) {
    const stdout = run_asm(asmOutput.value);
    programOutput.value = stdout.join("\n")
  }
}

function compileToAsm(code: string): string {
  const result = compileWeb(code)
  return result.fields[0]
}
</script>


<style scoped>
.playground {
  display: flex;
  flex-direction: row;
  height: 100vh;
  padding: 1rem;
  gap: 1rem;
  box-sizing: border-box;
}

.section {
  flex: 1;
  display: flex;
  flex-direction: column;
}

.code-input {
  font-family: monospace;
  font-size: 14px;
  width: 100%;
}

.button-row {
  margin-top: 0.5rem;
  display: flex;
  justify-content: flex-end;
}

.output-panel pre {
  margin: 0;
  font-family: monospace;
  font-size: 14px;
  background: #111;
  color: #0f0;
  padding: 0.5rem;
  overflow-y: auto;
  height: 100%;
}
</style>