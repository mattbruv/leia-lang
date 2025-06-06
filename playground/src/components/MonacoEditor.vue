<template>
  <div ref="container" class="editor"></div>
</template>

<script setup lang="ts">
import * as monaco from 'monaco-editor'
import { onMounted, onBeforeUnmount, ref, watch } from 'vue'

const props = defineProps<{
  code: string
}>()
const emit = defineEmits<{
  (e: 'update:code', value: string): void
}>()

const container = ref<HTMLDivElement | null>(null)
let editor: monaco.editor.IStandaloneCodeEditor | null = null

onMounted(() => {
  if (!container.value) return

  editor = monaco.editor.create(container.value, {
    value: props.code,
    language: 'leia',
    theme: 'vs-dark',
  })

  window.onresize = function () {
    editor?.layout()
  }

  editor.onDidChangeModelContent(() => {
    const value = editor?.getValue() ?? ''
    emit('update:code', value)
  })
})

watch(() => props.code, (newCode) => {
  if (editor && editor.getValue() !== newCode) {
    editor.setValue(newCode)
  }
})

onBeforeUnmount(() => {
  editor?.dispose()
})
</script>

<style scoped>
.editor {
  width: 100%;
  height: 100%;
}
</style>